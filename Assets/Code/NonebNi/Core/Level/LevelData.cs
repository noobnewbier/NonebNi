using NonebNi.Core.Maps;

namespace NonebNi.Core.Level
{
    public class LevelData
    {
        public MapConfigData MapConfig { get; }
        public WorldConfigData WorldConfig { get; }

        public LevelData(MapConfigData mapConfig, WorldConfigData worldConfig)
        {
            MapConfig = mapConfig;
            WorldConfig = worldConfig;
        }
    }
}