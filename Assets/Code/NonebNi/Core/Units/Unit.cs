using Noneb.Core.Game.Common.BoardItems;
using Noneb.Core.Game.Common.TagInterface;
using Noneb.Core.Game.Coordinates;

namespace Noneb.Core.Game.Units
{
    public class Unit : BoardItem<UnitData>, IOnTile
    {
        public Unit(UnitData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}