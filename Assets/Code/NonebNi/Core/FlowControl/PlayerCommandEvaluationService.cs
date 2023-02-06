using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.Maps;
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
        private readonly ICommandHandler<TeleportCommand> _teleportCommandHandler;
        private readonly ICommandHandler<EndTurnCommand> _endTurnCommandHandler;
        private readonly ICommandHandler<MoveUnitCommand> _moveCommandHandler;

        public CommandEvaluationService(IMap map)
        {
            //TODO: inject the command handler
            _damageCommandHandler = new DamageCommandHandler(map);
            _teleportCommandHandler = new TeleportCommandHandler(map);
            _endTurnCommandHandler = new EndTurnCommandHandler();
            _moveCommandHandler = new MoveUnitCommandHandler(map);
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