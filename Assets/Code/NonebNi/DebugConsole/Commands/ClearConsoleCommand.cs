using JetBrains.Annotations;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("clear", "Clear accumulated output in the console")]
    [UsedImplicitly]
    public class ClearConsoleCommand : IConsoleCommand { }
}