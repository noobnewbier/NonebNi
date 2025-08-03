using System.Collections.Generic;
using System.Linq;
using Noneb.Localization.Runtime;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.FlowControl;
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
        public enum ErrorType
        {
            Unknown,
            CannotPayCost,
            InvalidTarget,
            OutOfRange
        }

        (Error? error, ICommand command) ValidateDecision(IDecision? decision);

        (bool canBeValid, Error? error) ValidateDecisionConstructionInput(NonebAction action, EntityData caster, IReadOnlyList<Coordinate> existingInput, Coordinate newInput);

        /// <summary>
        ///     Describe why a decision is invalid.
        /// </summary>
        public class Error
        {
            public readonly NonebLocString Description;

            public readonly ErrorType Type;

            public Error(ErrorType type, NonebLocString description)
            {
                Type = type;
                Description = description;
            }

            public static Error Unknown { get; } = new(ErrorType.Unknown, "Failed for an undefined reason");
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
                                IDecisionValidator.ErrorType.CannotPayCost,
                                $"{ad.Action.Name} cost more than what the {ad.ActorEntity} can pay for"
                            ),
                            NullCommand.Instance
                        );

                    if (!IsTargetingValid(ad))
                    {
                        return (
                            new IDecisionValidator.Error(
                                IDecisionValidator.ErrorType.InvalidTarget,
                                $"action {ad.Action.Name} cannot be targeted at {ad.TargetCoords}"
                            ),
                            NullCommand.Instance
                        );
                    }

                    if (_gameEventControl.ActiveActionResult.CanCombo)
                        if (!IsTargetingComboTarget(ad) && !IsStartingFromComboCarrier(ad))
                            return (
                                new IDecisionValidator.Error(
                                    IDecisionValidator.ErrorType.InvalidTarget,
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

        //todo: instead of checking an input, it might be easier to check in bulk, that way we don't have to rework our code. the front cost might be expensive but hopefully ain't too bad.
        //we can also you know, just cache the fucker?
        public (bool canBeValid, IDecisionValidator.Error? error) ValidateDecisionConstructionInput(NonebAction action, EntityData caster, IReadOnlyList<Coordinate> existingInput, Coordinate newInput)
        {
            /*
             * Note:
             * Since this method is, quite literally, probably going to be called every frame,
             * we probably want to improve the efficiency by caching *something*.
             *
             * We can in theory turn this in to an async func that slowly takes more input(or just make it a separate class dude, we need to handle backing in UI as well).
             *
             * But hey we also shouldn't optimize without profiling so fuck it future me it's your job.
             */
            var requests = action.TargetRequests;
            var request = requests[existingInput.Count];

            var (canBeValid, error) = RangeCheck();
            if (existingInput.Count < requests.Length - 1)
                // normally we only check for range
                return (canBeValid, error);

            // but at the last step we want to check for combo constraint as well
            if (!canBeValid)
                // don't even check for combo - we are invalid just for the range
                return (false, error);

            // construct a decision, and do a usual rundown to check everything(include combos), not the most efficient but will do for now.
            var decision = new ActionDecision(action, caster, existingInput.Append(newInput));
            var (decisionError, _) = ValidateDecision(decision);
            if (decisionError != null) return (false, decisionError);

            return (true, null);

            (bool canBeValid, IDecisionValidator.Error? error) RangeCheck()
            {
                var ranges = _targetFinder.FindRange(caster, request).ToArray();
                var (status, _) = ranges.FirstOrDefault(t => t.coord == newInput);
                if (status == null)
                    // it's so far out the target finder don't even think it should be in the list it returns -> is there a more elegant way?
                    return (false, new IDecisionValidator.Error(IDecisionValidator.ErrorType.OutOfRange, nameof(RangeStatus.OutOfRange)));

                switch (status)
                {
                    case RangeStatus.Targetable:
                        return (true, null);

                    case RangeStatus.OutOfRange:
                        return (false, new IDecisionValidator.Error(IDecisionValidator.ErrorType.OutOfRange, status.GetType().Name));

                    case RangeStatus.InRangeButNoTarget:
                    case RangeStatus.NotTargetable:
                        return (false, new IDecisionValidator.Error(IDecisionValidator.ErrorType.InvalidTarget, status.GetType().Name));

                    default:
                        Log.Error($"Unhandled type {status}");
                        return (false, new IDecisionValidator.Error(IDecisionValidator.ErrorType.InvalidTarget, status.GetType().Name));
                }
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