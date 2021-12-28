using NonebNi.Core.Level;

namespace NonebNi.Ui.Di
{
    public class GameModule
    {
        public LevelData LevelData { get; }

        public GameModule(LevelData levelData)
        {
            LevelData = levelData;
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            new CoordinateAndPositionService(LevelData.WorldConfig);
    }
}