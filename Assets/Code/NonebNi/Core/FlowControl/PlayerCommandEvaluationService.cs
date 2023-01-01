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
        private readonly DamageCommandHandler _damageCommandHandler;
        private readonly TeleportCommandHandler _teleportCommandHandler;

        public CommandEvaluationService(IMap map)
        {
            //TODO: inject the command handler
            _damageCommandHandler = new DamageCommandHandler(map);
            _teleportCommandHandler = new TeleportCommandHandler(map);
        }

        public IEnumerable<ISequence> Evaluate(ICommand command)
        {
            return command switch
            {
                DamageCommand damageCommand => _damageCommandHandler.Evaluate(damageCommand),
                TeleportCommand teleportCommand => _teleportCommandHandler.Evaluate(teleportCommand),
                _ => Enumerable.Empty<ISequence>()
            };
        }
    }
}