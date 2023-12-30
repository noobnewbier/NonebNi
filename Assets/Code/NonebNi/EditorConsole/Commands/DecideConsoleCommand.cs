using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("decide", "make a decision for the current actor")]
    [UsedImplicitly]
    public class DecideConsoleCommand : IConsoleCommand
    {
        public readonly string ActionId;
        public readonly Coordinate[] TargetCoords;

        public DecideConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the target")]
            Coordinate targetCoord)
        {
            TargetCoords = new[] { targetCoord };
            ActionId = actionId;
        }

        public DecideConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the target")]
            params Coordinate[] targetCoords)
        {
            TargetCoords = targetCoords;
            ActionId = actionId;
        }
    }
}