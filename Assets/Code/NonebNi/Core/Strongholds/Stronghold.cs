using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Strongholds
{
    public class Stronghold : BoardItem<StrongholdData>, IOnTile
    {
        public Stronghold(StrongholdData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}