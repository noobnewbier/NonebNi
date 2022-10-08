using JetBrains.Annotations;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("help",
        "Print help message for a specific command, or list all available commands if no command name is provided")]
    public class HelpCommand : IConsoleCommand
    {
        public readonly string? CommandName;

        [SignatureInfo("Print help message for the specified command name")]
        public HelpCommand([CommandParam("The command alias to search for")] string commandName)
        {
            CommandName = commandName;
        }

        [UsedImplicitly]
        [SignatureInfo("Print a list of all available commands and their descriptions")]
        public HelpCommand()
        {
        }
    }
}