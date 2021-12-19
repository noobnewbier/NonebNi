using NonebNi.Editors.Level.Data;

namespace NonebNi.Editors.Level
{
    public class LevelEditorModel
    {
        public EditorLevelData EditorLevelData { get; }
        public LevelDataSource LevelDataSource { get; }
        public EditorMap EditorMap { get; }

        public LevelEditorModel(EditorLevelData editorLevelData, EditorMap editorMap, LevelDataSource levelDataSource)
        {
            EditorLevelData = editorLevelData;
            EditorMap = editorMap;
            LevelDataSource = levelDataSource;
        }
    }
}