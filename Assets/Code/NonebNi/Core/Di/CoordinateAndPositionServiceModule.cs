using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;

namespace NonebNi.Core.Di
{
    public class CoordinateAndPositionServiceModule
    {
        public CoordinateAndPositionService GetCoordinateAndPositionService(LevelData levelData) =>
            new CoordinateAndPositionService(levelData.WorldConfig);
    }
}