using Noneb.Core.Game.Common.BoardItems;
using Noneb.Core.Game.Coordinates;

namespace Noneb.Core.Game.Tiles
{
    public class Tile : BoardItem<TileData>
    {
        public Tile(TileData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}