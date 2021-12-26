using NonebNi.Game.Level;

namespace NonebNi.Game.Di
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