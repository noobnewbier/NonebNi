using System;
using NonebNi.Core.Level;

namespace NonebNi.Editor.Maps
{
    /// <summary>
    /// Maybe we can do some source code weaving for this ungodly boilerplate?
    /// That will probably have to be a standalone project.
    /// </summary>
    public class LevelEditorDataModel
    {
        private bool _isGizmosVisible;
        private bool _isGridVisible;
        private LevelData? _levelData;

        public bool IsGridVisible
        {
            get => _isGridVisible;
            set
            {
                _isGridVisible = value;
                OnGridVisibilityChanged?.Invoke(_isGridVisible);
            }
        }

        public bool IsGizmosVisible
        {
            get => _isGizmosVisible;
            set
            {
                _isGizmosVisible = value;
                OnGizmosVisibilityChanged?.Invoke(_isGizmosVisible);
            }
        }

        public LevelData? CurrentLevelData
        {
            get => _levelData;
            set
            {
                _levelData = value;
                OnLevelDataChanged?.Invoke(_levelData);
            }
        }

        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnGizmosVisibilityChanged;
        public event Action<LevelData?>? OnLevelDataChanged;
    }
}