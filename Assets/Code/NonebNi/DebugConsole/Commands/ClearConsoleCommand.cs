using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("clear", "Clear accumulated output in the console")]
    [UsedImplicitly]
    public class ClearConsoleCommand : IConsoleCommand { }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(ClearConsoleCommand _, StringBuilder outputBuffer)
        {
            outputBuffer.Clear();
            return UniTask.CompletedTask;
        }
    }
}