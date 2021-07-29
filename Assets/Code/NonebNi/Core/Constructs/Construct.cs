using Noneb.Core.Game.Common.BoardItems;
using Noneb.Core.Game.Common.TagInterface;
using Noneb.Core.Game.Coordinates;

namespace Noneb.Core.Game.Constructs
{
    public class Construct : BoardItem<ConstructData>, IOnTile
    {
        public Construct(ConstructData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}