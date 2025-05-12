using System.Collections.Generic;
using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Commands
{
    public class ActionCommand : ICommand
    {
        public readonly NonebAction Action;

        public readonly EntityData ActorEntity;

        public readonly IReadOnlyList<Coordinate> TargetCoords;

        public ActionCommand(NonebAction action, EntityData actorEntity, params Coordinate[] targetCoords)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoords = targetCoords;
        }
    }
}