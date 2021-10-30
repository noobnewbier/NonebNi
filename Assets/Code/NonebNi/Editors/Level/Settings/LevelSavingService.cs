using NonebNi.Core.Level;

namespace NonebNi.Editors.Level.Settings
{
    public class LevelSavingService
    {
        private readonly LevelDataSource _dataSource;
        private readonly LevelData _levelData;

        public LevelSavingService(LevelDataSource dataSource, LevelData levelData)
        {
            _dataSource = dataSource;
            _levelData = levelData;
        }

        public void Save()
        {
            _dataSource.CopyFromData(_levelData);
        }
    }
}