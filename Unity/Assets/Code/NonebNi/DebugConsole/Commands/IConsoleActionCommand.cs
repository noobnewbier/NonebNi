using NonebNi.Core.Actions;

namespace NonebNi.DebugConsole.Commands
{
    public interface IConsoleActionCommand : IConsoleCommand
    {
        NonebAction GetAction();
    }
}