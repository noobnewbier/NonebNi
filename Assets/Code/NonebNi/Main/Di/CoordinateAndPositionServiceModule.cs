using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using StrongInject;

namespace NonebNi.Main.Di
{
    public class CoordinateAndPositionServiceModule
    {
        [Factory]
        public static ICoordinateAndPositionService GetCoordinateAndPositionService(WorldConfigData worldConfig) =>
            new CoordinateAndPositionService(worldConfig);
    }
}