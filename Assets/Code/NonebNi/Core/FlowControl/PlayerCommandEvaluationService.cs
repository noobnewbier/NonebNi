using System;
using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.FlowControl
{
    public interface ICommandEvaluationService
    {
        IEnumerable<ISequence> Evaluate(ICommand command);
    }

    public class CommandEvaluationService : ICommandEvaluationService
    {
        private readonly ICommandHandler<DamageCommand> _damageCommandHandler;
        private readonly ICommandHandler<EndTurnCommand> _endTurnCommandHandler;
        private readonly ICommandHandler<MoveUnitCommand> _moveCommandHandler;
        private readonly ICommandHandler<TeleportCommand> _teleportCommandHandler;

        public CommandEvaluationService(
            ICommandHandler<DamageCommand> damageCommandHandler,
            ICommandHandler<TeleportCommand> teleportCommandHandler,
            ICommandHandler<EndTurnCommand> endTurnCommandHandler,
            ICommandHandler<MoveUnitCommand> moveCommandHandler)
        {
            _damageCommandHandler = damageCommandHandler;
            _teleportCommandHandler = teleportCommandHandler;
            _endTurnCommandHandler = endTurnCommandHandler;
            _moveCommandHandler = moveCommandHandler;
        }

        public IEnumerable<ISequence> Evaluate(ICommand command)
        {
            return command switch
            {
                DamageCommand damageCommand => _damageCommandHandler.Evaluate(damageCommand),
                TeleportCommand teleportCommand => _teleportCommandHandler.Evaluate(teleportCommand),
                EndTurnCommand endTurnCommand => _endTurnCommandHandler.Evaluate(endTurnCommand),
                MoveUnitCommand moveUnitCommand => _moveCommandHandler.Evaluate(moveUnitCommand),

                _ => throw new NotImplementedException("Something went wrong - unexpected type")
            };
        }
    }
}