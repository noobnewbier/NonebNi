using System;
using NonebNi.Core.Level;

namespace NonebNi.Editor.Level
{
    /// <summary>
    /// Maybe we can do some source code weaving for this ungodly boilerplate?
    /// That will probably have to be a standalone project.
    /// </summary>
    public class LevelEditorDataModel
    {
        private bool _isGizmosVisible;
        private bool _isGridVisible;
        public LevelData LevelData { get; }

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

        public LevelEditorDataModel(LevelData levelData)
        {
            LevelData = levelData;
        }

        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnGizmosVisibilityChanged;
    }
}