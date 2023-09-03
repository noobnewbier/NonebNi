﻿using JetBrains.Annotations;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Commands.Attributes;

namespace NonebNi.EditorConsole.Commands
{
    [Command("action", "cast an action from the actor to the target tile")]
    [UsedImplicitly]
    public class ActionConsoleCommand : IConsoleCommand
    {
        public readonly string ActionId;
        public readonly Coordinate? ActorCoord;
        public readonly Coordinate TargetCoord;

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the target")]
            Coordinate targetCoord)
        {
            ActorCoord = null;
            TargetCoord = targetCoord;
            ActionId = actionId;
        }

        public ActionConsoleCommand(
            [CommandParam("the ID of the casted action")]
            string actionId,
            [CommandParam("coordinate of the actor, if left empty, we will use the current unit to perform the action")]
            Coordinate actorCoord,
            [CommandParam("coordinate of the target")]
            Coordinate targetCoord)
        {
            ActorCoord = actorCoord;
            TargetCoord = targetCoord;
            ActionId = actionId;
        }
    }
}