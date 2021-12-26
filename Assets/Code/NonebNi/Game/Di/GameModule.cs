using Code.NonebNi.Game.Level;

namespace Code.NonebNi.Game.Di
{
    public class GameModule
    {
        public LevelData LevelData { get; }

        public GameModule(LevelData levelData)
        {
            LevelData = levelData;
        }
    }
}