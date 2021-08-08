using NonebNi.Core.Maps;

namespace NonebNi.Core.Level
{
    public class LevelData
    {
        public MapConfig MapConfig { get; }
        public WorldConfig WorldConfig { get; }
        public Map Map { get; }

        public LevelData(MapConfig mapConfig, WorldConfig worldConfig, Map map)
        {
            MapConfig = mapConfig;
            WorldConfig = worldConfig;
            Map = map;
        }
    }
}