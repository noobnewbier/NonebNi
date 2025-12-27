using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Decisions;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command(
        "move",
        "Send a move decision for the active unit, the game proceeds as normal after the decision is sent. Note this simply make a decision on the AI/Player behalf - aka all rules still applies"
    )]
    [UsedImplicitly]
    public class MoveConsoleCommand : IConsoleCommand
    {
        public readonly Coordinate TargetPos;

        public MoveConsoleCommand([CommandParam("The target position of the unit after moving")] Coordinate targetPos)
        {
            TargetPos = targetPos;
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(MoveConsoleCommand command, StringBuilder _)
        {
            _agentsService.OverrideDecision(
                new ActionDecision(
                    ActionDatas.Move,
                    _turnOrderer.CurrentUnit,
                    command.TargetPos
                )
            );

            return UniTask.CompletedTask;
        }
    }
}