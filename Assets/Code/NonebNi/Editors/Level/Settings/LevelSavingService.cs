using NonebNi.Editors.Level.Data;

namespace NonebNi.Editors.Level.Settings
{
    public class LevelSavingService
    {
        private readonly LevelDataSource _dataSource;
        private readonly EditorLevelData _editorLevelData;

        public LevelSavingService(LevelDataSource dataSource, EditorLevelData editorLevelData)
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