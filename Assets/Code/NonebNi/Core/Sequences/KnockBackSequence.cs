using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;

namespace NonebNi.Core.Sequences
{
    public class KnockBackSequence : ISequence
    {
        public readonly EntityData MovedUnit;
        public readonly Coordinate TargetCoord;

        public KnockBackSequence(EntityData movedUnit, Coordinate targetCoord)
        {
            MovedUnit = movedUnit;
            TargetCoord = targetCoord;
        }
    }
}