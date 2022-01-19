using NonebNi.LevelEditor.Level.Data;

namespace NonebNi.LevelEditor.Level.Settings
{
    public class LevelSavingService
    {
        private readonly EditorLevelDataSource _dataSource;
        private readonly EditorLevelData _editorLevelData;

        public LevelSavingService(EditorLevelDataSource dataSource, EditorLevelData editorLevelData)
        {
            _dataSource = dataSource;
            _editorLevelData = editorLevelData;
        }

        public void Save()
        {
            _dataSource.CopyFromData(_editorLevelData);
        }
    }
}