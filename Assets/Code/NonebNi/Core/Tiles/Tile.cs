using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Tiles
{
    public class Tile : BoardItem<TileData>
    {
        public Tile(TileData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}