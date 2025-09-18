using NonebNi.Core.Debug;
using NonebNi.DebugConsole;

namespace NonebNi.Main
{
    public class DebugTools
    {
        public readonly NonebDebugConsole Console;
        public readonly IDebugFlagRepository DebugFlagRepository;

        public DebugTools(NonebDebugConsole console, IDebugFlagRepository debugFlagRepository)
        {
            Console = console;
            DebugFlagRepository = debugFlagRepository;
        }
    }
}