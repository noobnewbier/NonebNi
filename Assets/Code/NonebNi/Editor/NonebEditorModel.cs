using System;
using NonebNi.Core.Level;

namespace NonebNi.Editor
{
    public class NonebEditorModel
    {
        private bool _isGizmosVisible;
        private bool _isGridVisible;
        private LevelDataSource? _levelDataSource;

        public bool IsGridVisible
        {
            get => _isGridVisible;
            set
            {
                if (!_isGizmosVisible)
                {
                    _isGridVisible = value;
                    OnGridVisibilityChanged?.Invoke(_isGridVisible);
                }
            }
        }

        public bool IsGizmosVisible
        {
            get => _isGizmosVisible;
            set
            {
                if (_isGizmosVisible != value)
                {
                    _isGizmosVisible = value;
                    OnGizmosVisibilityChanged?.Invoke(_isGizmosVisible);
                }
            }
        }

        public LevelDataSource? LevelDataSource
        {
            get => _levelDataSource;
            set
            {
                if (_levelDataSource != value)
                {
                    _levelDataSource = value;
                    OnLevelDataSourceChanged?.Invoke(_levelDataSource);
                }
            }
        }


        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnGizmosVisibilityChanged;
        public event Action<LevelDataSource?>? OnLevelDataSourceChanged;
    }
}