using NonebNi.Core.BoardItems;

namespace NonebNi.Core.Tiles
{
    public class TileData : BoardItemData
    {
        public TileData(string name, float weight) : base(name)
        {
            Weight = weight;
        }

        public float Weight { get; }
    }
}