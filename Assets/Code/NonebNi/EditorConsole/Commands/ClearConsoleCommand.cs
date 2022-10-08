using JetBrains.Annotations;

namespace NonebNi.EditorConsole.Commands
{
    [Command("clear", "Clear accumulated output in the console")]
    [UsedImplicitly]
    public class ClearConsoleCommand : IConsoleCommand
    {
    }
}