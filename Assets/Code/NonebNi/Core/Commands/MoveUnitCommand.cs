using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    public class MoveUnitCommand : ICommand
    {
        public readonly Coordinate EndCoord;
        public readonly UnitData MovedUnit;

        public MoveUnitCommand(UnitData movedUnit, Coordinate endCoord)
        {
            MovedUnit = movedUnit;
            EndCoord = endCoord;
        }
    }
}