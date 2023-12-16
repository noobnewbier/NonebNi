﻿using NonebNi.Core.Actions;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Decisions
{
    public class ActionDecision : IDecision
    {
        public readonly NonebAction Action;
        public readonly EntityData ActorEntity;
        public readonly Coordinate TargetCoord;

        public ActionDecision(NonebAction action, EntityData actorEntity, Coordinate targetCoord)
        {
            Action = action;
            ActorEntity = actorEntity;
            TargetCoord = targetCoord;
        }
    }
}