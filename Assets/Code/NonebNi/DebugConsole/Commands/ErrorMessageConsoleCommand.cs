using System.Text;
using Cysharp.Threading.Tasks;
using NonebNi.DebugConsole.Commands;

namespace NonebNi.DebugConsole.Commands
{
    public class ErrorMessageConsoleCommand : IConsoleCommand
    {
        public ErrorMessageConsoleCommand(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(ErrorMessageConsoleCommand command, StringBuilder outputBuffer)
        {
            outputBuffer.Append(command.Message);
            outputBuffer.AppendLine();

            return UniTask.CompletedTask;
        }
    }
}