using NonebNi.LevelEditor.Level.Data;

namespace NonebNi.LevelEditor.Level
{
    public class LevelEditorModel
    {
        public EditorLevelData EditorLevelData { get; }
        public EditorLevelDataSource EditorLevelDataSource { get; }
        public EditorMap EditorMap { get; }

        public LevelEditorModel(EditorLevelData editorLevelData,
                                EditorMap editorMap,
                                EditorLevelDataSource editorLevelDataSource)
        {
            EditorLevelData = editorLevelData;
            EditorMap = editorMap;
            EditorLevelDataSource = editorLevelDataSource;
        }
    }
}