using System.Collections.Generic;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Commands.Handlers
{
    public class EndTurnCommandHandler : ICommandHandler<EndTurnCommand>
    {
        public IEnumerable<ISequence> Evaluate(EndTurnCommand command)
        {
            yield break;
        }
    }
}