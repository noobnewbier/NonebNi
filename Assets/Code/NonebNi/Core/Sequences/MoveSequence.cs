﻿using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Sequences
{
    public class MoveSequence : ISequence
    {
        public readonly EntityData MovedEntity;
        public readonly Coordinate TargetCoord;

        public MoveSequence(EntityData movedEntity, Coordinate targetCoord)
        {
            TargetCoord = targetCoord;
            MovedEntity = movedEntity;
        }
    }
}