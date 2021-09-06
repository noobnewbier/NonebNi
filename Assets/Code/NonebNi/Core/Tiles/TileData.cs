using NonebNi.Core.Entities;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    /// Todo: refactor such that tile is no longer an entity -> this seems inherently off.
    /// </summary>
    public class TileData : EntityData
    {
        public float Weight { get; }

        public TileData(string name, float weight) : base(name)
        {
            Weight = weight;
        }
    }
}