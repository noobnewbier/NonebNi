using NonebNi.Core.Entities;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    /// Todo: refactor such that tile is no longer an entity -> this seems inherently off... or is it?
    /// </summary>
    public class TileData : EntityData
    {
        public static readonly TileData Default = new TileData("Default", 1);
        public float Weight { get; }

        public TileData(string name, float weight) : base(name)
        {
            Weight = weight;
        }
    }
}