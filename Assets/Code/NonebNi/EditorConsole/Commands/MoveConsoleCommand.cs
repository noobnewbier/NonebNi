using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("move",
        "Send a move decision for the active unit, the game proceeds as normal after the decision is sent. Note this simply make a decision on the AI/Player behalf - aka all rules still applies")]
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