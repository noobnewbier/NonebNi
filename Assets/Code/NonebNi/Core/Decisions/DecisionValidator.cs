using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;

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
        private readonly IActionCommandEvaluator _commandEvaluator;
        private readonly IGameEventControl _gameEventControl;
        private readonly IReadOnlyMap _map;
        private readonly ITargetFinder _targetFinder;

        public DecisionValidator(IReadOnlyMap map, ITargetFinder targetFinder, IActionCommandEvaluator commandEvaluator, IGameEventControl gameEventControl)
        {
            _map = map;
            _targetFinder = targetFinder;
            _commandEvaluator = commandEvaluator;
            _gameEventControl = gameEventControl;
        }

        public (IDecisionValidator.Error? error, ICommand command) ValidateDecision(IDecision? decision)
        {
            switch (decision)
            {
                case EndTurnDecision:
                    return (null, new EndTurnCommand());
                case ActionDecision ad:
                {
                    var cost = _commandEvaluator.FindActionCostInCurrentState(ad.Action);
                    if (!ad.ActorEntity.CanPayCosts(cost))
                        return (
                            new IDecisionValidator.Error(
                                "cannot-pay-cost",
                                $"{ad.Action.Name} cost more than what the {ad.ActorEntity} can pay for"
                            ),
                            NullCommand.Instance
                        );

                    if (!IsTargetingValid(ad))
                    {
                        return (
                            new IDecisionValidator.Error(
                                "invalid-target",
                                $"action {ad.Action.Name} cannot be targeted at {ad.TargetCoords}"
                            ),
                            NullCommand.Instance
                        );
                    }

                    if (_gameEventControl.ActiveActionResult.CanCombo)
                        if (!IsTargetingComboTarget(ad) && !IsStartingFromComboCarrier(ad))
                            return (
                                new IDecisionValidator.Error(
                                    "invalid-target",
                                    "You must be targeting the combo target or start with the combo carrier"
                                ),
                                NullCommand.Instance
                            );

                    return (null, new ActionCommand(ad.Action, ad.ActorEntity, ad.TargetCoords));
                }
                default:
                    return (IDecisionValidator.Error.Unknown, NullCommand.Instance);
            }
        }

        private bool IsStartingFromComboCarrier(ActionDecision ad)
        {
            return _gameEventControl.ActiveActionResult.ValidComboCarrier.Any(c => c == ad.ActorEntity);
        }

        private bool IsTargetingComboTarget(ActionDecision ad)
        {
            if (!_gameEventControl.ActiveActionResult.ValidComboReceiver.Any()) return false;

            var (isValidTargets, actionTargetGroups) = FindTargetFromInput(ad);
            if (!isValidTargets) return false;

            foreach (var targets in actionTargetGroups)
            {
                var targetGroupContainsReceiver = _gameEventControl.ActiveActionResult.ValidComboReceiver.Intersect(targets).Any();
                if (targetGroupContainsReceiver) return true;
            }

            return false;
        }

        private bool IsTargetingValid(ActionDecision ad)
        {
            var (isValidTargets, _) = FindTargetFromInput(ad);

            return isValidTargets;
        }

        private (bool, List<IActionTarget[]> toReturn) FindTargetFromInput(ActionDecision ad)
        {
            var action = ad.Action;
            var requirements = action.TargetRequests;
            var targetCoords = ad.TargetCoords;

            //targeted coordinates length must match restrictions length - otherwise we couldn't construct a valid command.
            //each targeted coordinate is validated against the restriction in the same index, so C0 -> R0, C1 -> R1 etc.
            //this is why the length must match, otherwise it doesn't make sense.
            //Before you ask, this is an arbitrary decision past you made, and another past you deduced, if you don't like it, well invent time machine.
            if (targetCoords.Length != requirements.Length) return (false, new List<IActionTarget[]>());

            //if actor is not on the map -> wtf are you doing.
            var actor = ad.ActorEntity;
            if (!_map.TryFind(actor, out Coordinate actorCoord)) return (false, new List<IActionTarget[]>());

            var toReturn = new List<IActionTarget[]>();
            for (var i = 0; i < targetCoords.Length; i++)
            {
                var requirement = requirements[i];
                var coord = targetCoords[i];
                var rangeLimit = requirement.Range.CalculateRange(actor);
                var distanceToTarget = actorCoord.DistanceTo(coord);

                // must meet all range limit for this to be valid
                if (distanceToTarget > rangeLimit) return (false, new List<IActionTarget[]>());

                var validTargets = _targetFinder.FindTargets(actor, coord, requirement.TargetArea, requirement.TargetRestrictionFlags)
                    .ToArray();

                //every targeted coordinate must have at least one valid target - otherwise it is an invalid command(can't target a coordinate without a target!).
                if (!validTargets.Any()) return (false, new List<IActionTarget[]>());

                toReturn.Add(validTargets);
            }

            return (true, toReturn);
        }
    }
}