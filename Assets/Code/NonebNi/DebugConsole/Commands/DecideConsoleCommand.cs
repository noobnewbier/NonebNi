using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command("decide", "make a decision for the current actor")]
    [UsedImplicitly]
    public class DecideConsoleCommand : IConsoleCommand
    {
        public readonly string ActionId;
        public readonly Coordinate[] TargetCoords;

        public DecideConsoleCommand(
            [CommandParam("the ID of the casted action")] string actionId,
            [CommandParam("coordinate of the target")] Coordinate targetCoord)
        {
            TargetCoords = new[] { targetCoord };
            ActionId = actionId;
        }

        public DecideConsoleCommand(
            [CommandParam("the ID of the casted action")] string actionId,
            [CommandParam("coordinate of the target")] params Coordinate[] targetCoords)
        {
            TargetCoords = targetCoords;
            ActionId = actionId;
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(DecideConsoleCommand command, StringBuilder outputBuffer)
        {
            var actionId = command.ActionId;
            var action = _actionRepository.GetAction(actionId);
            if (action == null)
            {
                outputBuffer.AppendLine(
                    $"Unable to find action with matching action ID: {actionId}"
                );
                return UniTask.CompletedTask;
            }

            _agentsService.OverrideDecision(
                new ActionDecision(
                    action,
                    _turnOrderer.CurrentUnit,
                    command.TargetCoords
                )
            );
            return UniTask.CompletedTask;
        }
    }
}