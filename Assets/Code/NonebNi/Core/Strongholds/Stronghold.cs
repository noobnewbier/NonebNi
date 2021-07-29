using Noneb.Core.Game.Common.BoardItems;
using Noneb.Core.Game.Common.TagInterface;
using Noneb.Core.Game.Coordinates;

namespace Noneb.Core.Game.Strongholds
{
    public class Stronghold : BoardItem<StrongholdData>, IOnTile
    {
        public Stronghold(StrongholdData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}