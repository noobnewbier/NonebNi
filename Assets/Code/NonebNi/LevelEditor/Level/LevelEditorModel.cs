using NonebNi.LevelEditor.Level.Maps;

namespace NonebNi.LevelEditor.Level
{
    public class LevelEditorModel
    {
        public LevelEditorModel(EditorLevelData editorLevelData,
            IEditorMap editorMap,
            EditorLevelDataSource editorLevelDataSource)
        {
            EditorLevelData = editorLevelData;
            EditorMap = editorMap;
            EditorLevelDataSource = editorLevelDataSource;
        }

        public EditorLevelData EditorLevelData { get; }
        public EditorLevelDataSource EditorLevelDataSource { get; }
        public IEditorMap EditorMap { get; }
    }
}