using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    public class NullCommand : ICommand
    {
        public static NullCommand Instance { get; } = new NullCommand();
    }
}