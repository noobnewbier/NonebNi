using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.FlowControl
{
    public interface ICommandEvaluationService
    {
        IEnumerable<ISequence> Evaluate(ICommand command);
    }

    public class CommandEvaluationService : ICommandEvaluationService, ICommandHandler<ComboCommand>
    {
        private readonly ICommandHandler<ActionCommand> _actionCommandHandler;
        private readonly ICommandHandler<EndTurnCommand> _endTurnCommandHandler;

        public CommandEvaluationService(
            ICommandHandler<EndTurnCommand> endTurnCommandHandler,
            ICommandHandler<ActionCommand> actionCommandHandler)
        {
            _endTurnCommandHandler = endTurnCommandHandler;
            _actionCommandHandler = actionCommandHandler;
        }

        public IEnumerable<ISequence> Evaluate(ICommand command)
        {
            return command switch
            {
                EndTurnCommand endTurnCommand => _endTurnCommandHandler.Evaluate(endTurnCommand),
                ActionCommand actionCommand => _actionCommandHandler.Evaluate(actionCommand),

                /*
                 * Special case:
                 * combo command handler simply needs to evaluate all command recursively,
                 * this means in theory an ICommandHandler<ComboCommand> would need the same set of commands that
                 * a CommandEvaluationService would need.
                 *
                 * Atm the only place that makes sense is to do it within this class.
                 * So we just make this CommandEvaluationService a command handler as well
                 */
                ComboCommand comboCommand => Evaluate(comboCommand),
                _ => throw new ArgumentOutOfRangeException(nameof(command), "Something went wrong - unexpected type")
            };
        }

        public IEnumerable<ISequence> Evaluate(ComboCommand command) => command.Commands.SelectMany(Evaluate);
    }
}