using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using UnityUtils;

namespace NonebNi.Core.Decisions
{
    public interface IActionOptionFinder
    {
        IEnumerable<ICommand> FindOptionsForActor(UnitData actorUnit);
        IEnumerable<ICommand> FindOptionsTargetingUnit(UnitData targetUnit, string actingFactionId);
    }

    public class ActionOptionFinder : IActionOptionFinder
    {
        private readonly IActionCommandEvaluator _commandEvaluator;
        private readonly IDecisionValidator _decisionValidator;
        private readonly IReadOnlyMap _map;
        private readonly ITargetFinder _targetFinder;

        public ActionOptionFinder(IReadOnlyMap map, IDecisionValidator decisionValidator, ITargetFinder targetFinder, IActionCommandEvaluator commandEvaluator)
        {
            _map = map;
            _decisionValidator = decisionValidator;
            _targetFinder = targetFinder;
            _commandEvaluator = commandEvaluator;
        }

        public IEnumerable<ICommand> FindOptionsForActor(UnitData actorUnit)
        {
            var actions = actorUnit.Actions;
            var affordableActions = actions.Where(actorUnit.CanPayActionCost);
            var toReturn = new List<ICommand>();

            // all possible action commands that an actor can do.
            foreach (var action in affordableActions)
            {
                var targetableRanges = action.TargetRequests
                    .Select(request => _targetFinder.FindRange(actorUnit, request))
                    .Select(range => range.Where(t => t.status is RangeStatus.Targetable))
                    .Select(range => range.Select(t => t.coord))
                    .Select(range => range.ToArray())
                    .ToArray();

                var rangeCombinations = targetableRanges.FindAllCombinations();
                var decisions = rangeCombinations.Select(coords => new ActionDecision(action, actorUnit, coords));
                var validCommands = decisions
                    .Select(d => _decisionValidator.ValidateDecision(d))
                    .Where(t => t.error == null)
                    .Select(t => t.command);

                toReturn.AddRange(validCommands);
            }

            // you can always do nothing
            toReturn.Add(new EndTurnCommand());

            return toReturn;
        }

        //todo: this is the sort of shit we need automated testing for 
        public IEnumerable<ICommand> FindOptionsTargetingUnit(UnitData targetUnit, string actingFactionId)
        {
            var allUnitsInFaction = _map.GetAllUnits().Where(u => u.FactionId == actingFactionId);
            foreach (var potentialActor in allUnitsInFaction)
            {
                var options = FindOptionsForActor(potentialActor);
                foreach (var option in options)
                    switch (option)
                    {
                        case EndTurnCommand:
                        case NullCommand:
                            continue;

                        case ActionCommand actionCommand:
                            var effectedTargets = _commandEvaluator.FindEffectedTargets(actionCommand);
                            if (effectedTargets.Any(g => g.Targets.Contains(targetUnit))) yield return option;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(option));
                    }
            }

            yield return new EndTurnCommand();
        }
    }
}