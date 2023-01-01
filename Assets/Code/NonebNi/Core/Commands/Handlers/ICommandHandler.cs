using System.Collections.Generic;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Commands.Handlers
{
    public interface ICommandHandler<in T> where T : ICommand
    {
        IEnumerable<ISequence> Evaluate(T command);
    }
}