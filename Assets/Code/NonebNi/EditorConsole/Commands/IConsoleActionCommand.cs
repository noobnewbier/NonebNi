using NonebNi.Core.Actions;

namespace NonebNi.EditorConsole.Commands
{
    public interface IConsoleActionCommand : IConsoleCommand
    {
        NonebAction GetAction();
    }
}