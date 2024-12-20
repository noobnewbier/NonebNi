using System.Collections.Generic;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Sequences
{
    public class MoveSequence : ISequence
    {
        public readonly EntityData MovedEntity;
        public readonly IEnumerable<Coordinate> TargetCoords;

        public MoveSequence(EntityData movedEntity, Coordinate targetCoord)
        {
            TargetCoords = new[] { targetCoord };
            MovedEntity = movedEntity;
        }

        public MoveSequence(EntityData movedEntity, IEnumerable<Coordinate> targetCoords)
        {
            TargetCoords = targetCoords;
            MovedEntity = movedEntity;
        }
    }
}