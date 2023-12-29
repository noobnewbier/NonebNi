using JetBrains.Annotations;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Effects;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
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
            [CommandParam("The position of the targeted unit")]
            Coordinate startPos,
            [CommandParam("The target position of the unit after teleporting")]
            Coordinate targetPos)
        {
            StartPos = startPos;
            TargetPos = targetPos;
        }

        public NonebAction GetAction()
        {
            return new NonebAction(
                "debug-teleport",
                10000,
                new[] { TargetRestriction.NonOccupied },
                TargetArea.Single,
                0,
                new Effect[] { new MoveEffect() }
            );
        }
    }
}