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

        public bool IsInspectorVisible
        {
            get => EditorPrefs.GetBool(nameof(IsInspectorVisible));
            set
            {
                var existingValue = IsInspectorVisible;
                EditorPrefs.SetBool(nameof(IsInspectorVisible), value);
                if (existingValue != value) OnInspectorVisibilityChanged?.Invoke(value);
            }
        }

        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnInspectorVisibilityChanged;
    }
}