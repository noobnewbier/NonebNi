using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Decisions
{
    public class MoveDecision : IDecision
    {
        public readonly Coordinate EndCoord;
        public readonly UnitData MovedUnit;

        public MoveDecision(UnitData movedUnit, Coordinate endCoord)
        {
            MovedUnit = movedUnit;
            EndCoord = endCoord;
        }
    }
}