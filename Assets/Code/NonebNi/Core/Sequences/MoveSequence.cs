using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class MoveSequence : ISequence
    {
        public readonly UnitData MovedUnit;
        public readonly Coordinate UnitCommandTargetCoord;

        public MoveSequence(UnitData movedUnit, Coordinate unitCommandTargetCoord)
        {
            UnitCommandTargetCoord = unitCommandTargetCoord;
            MovedUnit = movedUnit;
        }
    }
}