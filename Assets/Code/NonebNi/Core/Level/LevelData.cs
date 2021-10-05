using NonebNi.Core.Maps;

namespace NonebNi.Core.Level
{
    public class LevelData
    {
        public WorldConfigData WorldConfig { get; }
        public Map Map { get; }

        public LevelData(WorldConfigData worldConfig, Map map)
        {
            WorldConfig = worldConfig;
            Map = map;
        }
    }
}