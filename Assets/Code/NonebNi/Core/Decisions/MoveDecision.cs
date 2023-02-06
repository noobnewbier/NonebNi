using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Decision
{
    public class MoveDecision : IDecision
    {
        public readonly UnitData MovedUnit;
        public readonly Coordinate EndCoord;

        public MoveDecision(UnitData movedUnit, Coordinate endCoord)
        {
            MovedUnit = movedUnit;
            EndCoord = endCoord;
        }
    }
}