using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;

namespace NonebNi.Core.Constructs
{
    public class Construct : BoardItem<ConstructData>, IOnTile
    {
        public Construct(ConstructData data, Coordinate coordinate) : base(data, coordinate)
        {
        }
    }
}