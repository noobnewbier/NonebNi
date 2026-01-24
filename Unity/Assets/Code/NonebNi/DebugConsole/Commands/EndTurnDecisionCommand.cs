using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Decisions;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("endTurn", "inflicts set amount of damage to unit on coordinate")]
    [UsedImplicitly]
    public class EndTurnDecisionCommand : IConsoleCommand { }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(EndTurnDecisionCommand _, [UsedImplicitly] StringBuilder __)
        {
            _agentsService.OverrideDecision(EndTurnDecision.Instance);
            return UniTask.CompletedTask;
        }
    }
}