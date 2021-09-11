using System;
using NonebNi.Core.Level;
using UnityEditor;

namespace NonebNi.Editor
{
    public class NonebEditorModel
    {
        private LevelDataSource? _levelDataSource;

        public bool IsGridVisible
        {
            get => EditorPrefs.GetBool(nameof(IsGridVisible));
            set
            {
                var existingValue = IsGridVisible;
                EditorPrefs.SetBool(nameof(IsGridVisible), value);
                if (existingValue != value) OnGridVisibilityChanged?.Invoke(value);
            }
        }

        public bool IsGizmosVisible
        {
            get => EditorPrefs.GetBool(nameof(IsGizmosVisible));
            set
            {
                var existingValue = IsGizmosVisible;
                EditorPrefs.SetBool(nameof(IsGizmosVisible), value);
                if (existingValue != value) OnGizmosVisibilityChanged?.Invoke(value);
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