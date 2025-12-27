using System.Text;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.Core.Units;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Commands.Attributes;

namespace NonebNi.DebugConsole.Commands
{
    [Command(
        "teleport",
        "teleport target to coordinate, regardless if there's a path between the start and target position"
    )]
    [UsedImplicitly]
    public class TeleportConsoleCommand : IConsoleActionCommand
    {
        public readonly Coordinate StartPos;
        public readonly Coordinate TargetPos;

        public TeleportConsoleCommand(
            [CommandParam("The position of the targeted unit")] Coordinate startPos,
            [CommandParam("The target position of the unit after teleporting")] Coordinate targetPos)
        {
            StartPos = startPos;
            TargetPos = targetPos;
        }

        public NonebAction GetAction()
        {
            return new NonebAction(
                "debug-teleport",
                0,
                0,
                10000,
                TargetArea.Single,
                TargetRestriction.NonOccupied,
                false,
                string.Empty,
                new MoveEffect()
            );
        }
    }
}

namespace NonebNi.DebugConsole
{
    public partial class CommandHandler
    {
        private UniTask DoHandle(TeleportConsoleCommand command, StringBuilder _)
        {
            if (_readOnlyMap.TryGet<UnitData>(command.StartPos, out var unit))
                EvaluateSequence(
                    new ActionCommand(command.GetAction(), unit, command.TargetPos)
                );

            return UniTask.CompletedTask;
        }
    }
}