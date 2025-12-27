using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;
using UnityUtils;

namespace NonebNi.DebugConsole.Commands
{
    [Command(
        "help",
        "Print help message for a specific command, or list all available commands if no command name is provided"
    )]
    public class HelpCommand : IConsoleCommand
    {
        public readonly string? CommandName;

        [SignatureInfo("Print help message for the specified command name")]
        public HelpCommand([CommandParam("The command alias to search for")] string commandName)
        {
            CommandName = commandName;
        }

        [UsedImplicitly, SignatureInfo("Print a list of all available commands and their descriptions")]
        public HelpCommand() { }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(HelpCommand command, StringBuilder outputBuffer)
        {
            if (string.IsNullOrWhiteSpace(command.CommandName))
                PrintHelpMessageForAllCommands();
            else if (_commandsDataRepository.TryGetCommand(command.CommandName, out var d))
                PrintHelpMessageForCommand(d);

            outputBuffer.AppendLine();

            void PrintHelpMessageForAllCommands()
            {
                foreach (var commandData in _commandsDataRepository.GetAllCommands())
                {
                    outputBuffer.Append(commandData.Name);
                    outputBuffer.AppendLine();
                    outputBuffer.Append(
                        $@"---
Command description:
    {commandData.Description}
"
                    );
                    outputBuffer.AppendLine();
                }
            }

            void PrintHelpMessageForCommand(CommandData commandData)
            {
                outputBuffer.Append(commandData.Name);
                outputBuffer.AppendLine();
                outputBuffer.Append(
                    $@"---
Command description:
    {commandData.Description}
"
                );
                foreach (var constructorInfo in commandData.CommandType.GetConstructors())
                {
                    var parameters = constructorInfo.GetParameters();
                    var commandFormat = parameters.Any() ?
                        string.Join(", ", parameters.Select(p => p.Name)) :
                        "NO_ARG";

                    outputBuffer.AppendLine($"Signature: [{commandFormat}]");

                    var signatureInfos = constructorInfo.GetAttribute<SignatureInfoAttribute>(false);
                    var signatureDescription = signatureInfos?.Description;
                    outputBuffer.AppendLine($"- Description: {signatureDescription}");

                    foreach (var parameter in parameters)
                    {
                        var commandParams =
                            parameter.GetAttributes<CommandParamAttribute>(false);
                        foreach (var paramInfo in commandParams)
                            outputBuffer.AppendLine($"   - {parameter.Name}: {paramInfo.Description}");
                    }

                    outputBuffer.AppendLine();
                }
            }

            return UniTask.CompletedTask;
        }
    }
}