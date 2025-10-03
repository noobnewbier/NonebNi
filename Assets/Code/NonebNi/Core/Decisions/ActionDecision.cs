using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Decisions
{
    public class ActionDecision : IDecision
    {
        public readonly NonebAction Action;
        public readonly EntityData ActorEntity;
        public readonly Coordinate[] TargetCoords;

        public ActionDecision(NonebAction action, EntityData actorEntity, IEnumerable<Coordinate> targetCoords) : this(action, actorEntity, targetCoords.ToArray()) { }

        public ActionDecision(ActionCommand command) : this(command.Action, command.ActorEntity, command.TargetCoords) { }

        public ActionDecision(NonebAction action, EntityData actorEntity, params Coordinate[] targetCoords)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoords = targetCoords;
        }
    }
}