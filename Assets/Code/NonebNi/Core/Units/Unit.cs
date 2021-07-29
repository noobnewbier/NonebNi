using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Units
{
    public class Unit : BoardItem<UnitData>, IOnTile
    {
        public Unit(UnitData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}