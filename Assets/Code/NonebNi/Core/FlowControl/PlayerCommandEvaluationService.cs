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

        public CommandEvaluationService(IMap map)
        {
            _damageCommandHandler = new DamageCommandHandler(map);
        }

        public IEnumerable<ISequence> Evaluate(ICommand command)
        {
            if (command is DamageCommand damageCommand)
            {
                return _damageCommandHandler.Evaluate(damageCommand);
            }
            
            return Enumerable.Empty<ISequence>();
        }
    }
}