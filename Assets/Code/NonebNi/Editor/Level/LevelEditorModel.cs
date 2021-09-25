using NonebNi.Core.Level;
using NonebNi.Core.Maps;

namespace NonebNi.Editor.Level
{
    /// <summary>
    /// Maybe we can do some source code weaving for this ungodly boilerplate?
    /// That will probably have to be a standalone project.
    /// </summary>
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