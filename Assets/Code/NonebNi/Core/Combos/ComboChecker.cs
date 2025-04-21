using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;

namespace NonebNi.Core.Combos
{
    public interface IComboChecker
    {
        IEnumerable<ICommand> FindComboOptions(ICommand command);
    }

    public class ComboChecker : IComboChecker
    {
        private readonly IActionOptionFinder _actionOptionFinder;
        private readonly IActionCommandEvaluator _commandEvaluator;

        public ComboChecker(IActionOptionFinder actionOptionFinder, IActionCommandEvaluator commandEvaluator)
        {
            _actionOptionFinder = actionOptionFinder;
            _commandEvaluator = commandEvaluator;
        }

        public IEnumerable<ICommand> FindComboOptions(ICommand command)
        {
            if (command is not ActionCommand actionCommand) yield break;

            var action = actionCommand.Action;
            if (!action.IsComboStarter) yield break;

            var actorFactionId = actionCommand.ActorEntity.FactionId;
            var effectedTargets = _commandEvaluator.FindEffectedTargets(actionCommand);
            foreach (var target in effectedTargets.SelectMany(group => group.Targets))
            {
                if (target is not UnitData targetedUnit) continue;

                if (targetedUnit.FactionId == actorFactionId)
                    // if target is your own faction -> find command that target can do.
                    foreach (var option in _actionOptionFinder.FindOptionsForActor(targetedUnit).OfType<ActionCommand>())
                        yield return option;
                else
                    // if target is enemy -> find command from friendly that targets it
                    foreach (var option in _actionOptionFinder.FindOptionsTargetingUnit(targetedUnit, actorFactionId).OfType<ActionCommand>())
                        yield return option;
            }

            // you can of course, always just end turn
            yield return new EndTurnCommand();
        }
    }
}