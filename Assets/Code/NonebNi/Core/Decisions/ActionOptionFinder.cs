using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Effects;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using UnityUtils;

namespace NonebNi.Core.Decisions
{
    public interface IActionOptionFinder
    {
        IEnumerable<ICommand> FindComboOptions(EffectContext context);
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

        public IEnumerable<ICommand> FindComboOptions(EffectContext context)
        {
            var command = context.Command;
            if (!command.Action.IsComboStarter) yield break;

            var actor = context.Command.ActorEntity;
            foreach (var target in context.TargetGroups.SelectMany(group => group.Targets))
            {
                if (target is not UnitData targetedUnit) continue;

                if (targetedUnit.FactionId == actor.FactionId)
                    // if target is your own faction -> find command that target can do.
                    foreach (var option in FindOptionsForActor(targetedUnit).OfType<ActionCommand>())
                        yield return option;
                else
                    // if target is enemy -> find command from friendly that targets it
                    foreach (var option in FindOptionsTargetingUnit(targetedUnit, actor.FactionId).OfType<ActionCommand>())
                        if (option.ActorEntity != actor)
                            yield return option;
            }
        }

        private IEnumerable<ICommand> FindOptionsForActor(UnitData actorUnit)
        {
            var actions = actorUnit.Actions;
            var affordableActions = actions.Where(
                a =>
                {
                    var cost = _commandEvaluator.FindActionCostInCurrentState(a);
                    return actorUnit.CanPayCosts(cost);
                }
            );
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

            return toReturn;
        }

        //todo: this is the sort of shit we need automated testing for 
        private IEnumerable<ICommand> FindOptionsTargetingUnit(UnitData targetUnit, string actingFactionId)
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
                            var context = _commandEvaluator.FindEffectContext(actionCommand);
                            if (context.TargetGroups.Any(g => g.Targets.Contains(targetUnit))) yield return option;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(option));
                    }
            }

            yield return new EndTurnCommand();
        }
    }
}