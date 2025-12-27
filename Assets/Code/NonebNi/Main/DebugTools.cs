using NonebNi.Core.Debug;
using NonebNi.Core.Maps;
using NonebNi.DebugConsole;
using NonebNi.Terrain;

namespace NonebNi.Main
{
    public class DebugTools
    {
        public readonly NonebDebugConsole Console;
        public readonly IDebugFlagRepository DebugFlagRepository;
        public readonly IReadOnlyMap Map;
        public readonly TerrainConfigData TerrainConfig;

        public DebugTools(NonebDebugConsole console, IDebugFlagRepository debugFlagRepository, IReadOnlyMap map, TerrainConfigData terrainConfig)
        {
            Console = console;
            DebugFlagRepository = debugFlagRepository;
            Map = map;
            TerrainConfig = terrainConfig;
        }
    }
}