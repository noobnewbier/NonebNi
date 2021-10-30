using NonebNi.Core.Level;
using NonebNi.Core.Maps;

namespace NonebNi.Editors.Level
{
    public class LevelEditorModel
    {
        public LevelData LevelData { get; }
        public LevelDataSource LevelDataSource { get; }
        public Map Map { get; }

        public LevelEditorModel(LevelData levelData, Map map, LevelDataSource levelDataSource)
        {
            LevelData = levelData;
            Map = map;
            LevelDataSource = levelDataSource;
        }
    }
}