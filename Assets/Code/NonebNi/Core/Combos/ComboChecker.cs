using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Commands;
using NonebNi.Core.Decisions;
using NonebNi.Core.Effects;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Units;

namespace NonebNi.Core.Combos
{
    public interface IComboChecker
    {
        IEnumerable<ICommand> FindComboOptions(ICommand command);
        IEnumerable<ICommand> FindComboOptions(EffectContext context);
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
            if (command is not ActionCommand actionCommand) return Enumerable.Empty<ICommand>();

            var context = _commandEvaluator.FindEffectContext(actionCommand);
            return FindComboOptions(context);
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
                    foreach (var option in _actionOptionFinder.FindOptionsForActor(targetedUnit).OfType<ActionCommand>())
                        yield return option;
                else
                    // if target is enemy -> find command from friendly that targets it
                    foreach (var option in _actionOptionFinder.FindOptionsTargetingUnit(targetedUnit, actor.FactionId).OfType<ActionCommand>())
                        if (option.ActorEntity != actor)
                            yield return option;
            }
        }
    }
}