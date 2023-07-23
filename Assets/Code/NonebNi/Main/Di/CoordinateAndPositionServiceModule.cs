using NonebNi.Terrain;
using StrongInject;

namespace NonebNi.Main.Di
{
    public class CoordinateAndPositionServiceModule
    {
        [Factory]
        public static ICoordinateAndPositionService GetCoordinateAndPositionService(TerrainConfigData terrainConfig) =>
            new CoordinateAndPositionService(terrainConfig);
    }
}