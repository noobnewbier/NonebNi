using NonebNi.Core.Maps;

namespace NonebNi.Core.Level
{
    public class LevelData
    {
        public MapConfigData MapConfig { get; }
        public WorldConfigData WorldConfig { get; }
        public Map Map { get; }

        public LevelData(MapConfigData mapConfig, WorldConfigData worldConfig, Map map)
        {
            MapConfig = mapConfig;
            WorldConfig = worldConfig;
            Map = map;
        }
    }
}