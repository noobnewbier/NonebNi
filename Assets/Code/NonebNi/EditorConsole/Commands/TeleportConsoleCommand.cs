using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("teleport",
        "teleport target to coordinate, regardless if there's a path between the start and target position")]
    [UsedImplicitly]
    public class TeleportConsoleCommand : IConsoleCommand
    {
        public readonly Coordinate StartPos;
        public readonly Coordinate TargetPos;

        public TeleportConsoleCommand([CommandParam("The position of the targeted unit")] Coordinate startPos,
            [CommandParam("The target position of the unit after teleporting")]
            Coordinate targetPos)
        {
            StartPos = startPos;
            TargetPos = targetPos;
        }
    }
}