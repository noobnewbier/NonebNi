using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("action", "cast an action from the actor to the target tile, bypassing decision validation")]
    [UsedImplicitly]
    public class ActionConsoleCommand : IConsoleCommand
    {
        public readonly string ActionId;
        public readonly Coordinate? ActorCoord;
        public readonly Coordinate[] TargetCoords;

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the target")]
            Coordinate targetCoord)
        {
            ActorCoord = null;
            TargetCoords = new[] { targetCoord };
            ActionId = actionId;
        }

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the target")]
            params Coordinate[] targetCoords)
        {
            ActorCoord = null;
            TargetCoords = targetCoords;
            ActionId = actionId;
        }

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the actor, if left empty, we will use the current unit to perform the action")]
            Coordinate actorCoord,
            [CommandParam("coordinate of the target")]
            params Coordinate[] targetCoords)
        {
            ActorCoord = actorCoord;
            TargetCoords = targetCoords;
            ActionId = actionId;
        }
    }
}