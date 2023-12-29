using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using Unity.Logging;

namespace NonebNi.Core.Decisions
{
    /// <summary>
    ///     Takes a <see cref="IDecision" /> and check if it's valid, if it is turns it into a <see cref="ICommand" />.
    ///     Difference between a command and a decision is that a decision can be invalid gameplay-wise,
    ///     this is because it's something (potentially) coming from a player and might not be valid at all.
    ///     In contrast all command, unless systematically impossible(multiple unit on the same spot, moving unit that doesn't
    ///     exist),
    ///     it will always happen, regardless if the command abides by the gameplay rule.
    /// </summary>
    public interface IDecisionValidator
    {
        (Error? error, ICommand command) ValidateDecision(IDecision? decision);

        /// <summary>
        ///     Describe why a decision is invalid.
        /// </summary>
        public class Error
        {
            public const string UnknownId = "unknown";

            public Error(string id, string description)
            {
                Id = id;
                Description = description;
            }

            public string Id { get; }
            public string Description { get; }

            public static Error Unknown { get; } = new(UnknownId, "Failed for an undefined reason");
        }
    }

    public class DecisionValidator : IDecisionValidator
    {
        private readonly IReadOnlyMap _map; //TODO: PathFinding
        private readonly ITargetFinder _targetFinder;

        public DecisionValidator(IReadOnlyMap map, ITargetFinder targetFinder)
        {
            _map = map;
            _targetFinder = targetFinder;
        }

        public (IDecisionValidator.Error? error, ICommand command) ValidateDecision(IDecision? decision)
        {
            switch (decision)
            {
                case EndTurnDecision:
                    return (null, new EndTurnCommand());
                case ActionDecision ad:
                    if (!IsValidActionDecision(ad))
                    {
                        return (
                            new IDecisionValidator.Error(
                                "invalid-target",
                                $"action {ad.Action} cannot be targeted at {ad.TargetCoords}"
                            ), new ActionCommand(ad.Action, ad.ActorEntity, ad.TargetCoords)); //TODO: validate    
                    }

                    return (null, new ActionCommand(ad.Action, ad.ActorEntity, ad.TargetCoords));
                default:
                    return (IDecisionValidator.Error.Unknown, NullCommand.Instance);
            }
        }

        private bool IsValidActionDecision(ActionDecision ad)
        {
            var action = ad.Action;
            var restrictions = action.TargetRestrictions;
            var targetCoords = ad.TargetCoords;

            //targeted coordinates length must match restrictions length - otherwise we couldn't construct a valid command.
            if (targetCoords.Length != restrictions.Length) return false;

            //if actor is not on the map -> wtf are you doing.
            var actor = ad.ActorEntity;
            if (!_map.TryFind(actor, out Coordinate actorCoord)) return false;

            //if any of the target coords is out of range -> this is invalid.
            var range = action.Range.CalculateRange(actor);
            var distToTargets = targetCoords.Select(c => actorCoord.DistanceTo(c));
            if (distToTargets.Any(d => d > range)) return false;

            //every targeted coordinate must have at least one valid target - otherwise it is an invalid command(can't target a coordinate without a target!).
            for (var i = 0; i < targetCoords.Length; i++)
            {
                var coord = targetCoords[i];
                var restriction = restrictions[i];
                var validTargets = _targetFinder.FindTargets(actor, coord, action.TargetArea, restriction)
                    .ToArray();

                if (!validTargets.Any()) return false;

                if (validTargets.Length > 1)
                    /*
                     * NOTE:
                     * At the moment, there's a risk where if:
                     * 1. a coordinate have multiple valid target
                     * 2. the action is only valid for a single target - imagine if you a "slashing" a single entity
                     *
                     * The current check won't suffice as in this case having more than one valid target becomes invalid.
                     * What I really want to do, I think, is to have the input give me a list of the actual target instead of giving me a bunch of coordinate?
                     * Hard to tell but for now this suffice.
                     * ---
                     * Had another think when I came back to this the a few days later.
                     * I have a feeling that the core of the issue is that we are changing our "core" to work with the user interface,
                     * maybe this is why we are taking a coordinate as an input instead of an IActionTarget, which led to this weird gymnastic...
                     */
                    Log.Warning(
                        "More than one valid targets, it's most likely not what you expect when you are writing this - you were taking shortcuts and decided not to fix this issue right now, refer to the comment for more details"
                    );
            }

            return true;
        }
    }
}