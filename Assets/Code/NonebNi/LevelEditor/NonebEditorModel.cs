using System;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Tiles;
using UnityEditor;

namespace NonebNi.LevelEditor
{
    public class NonebEditorModel
    {
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

        /// <summary>
        /// "HelperWindows" include both <see cref="TileInspectorView" /> and <see cref="ErrorOverviewView" />
        /// </summary>
        public bool IsHelperWindowsVisible
        {
            get => EditorPrefs.GetBool(nameof(IsHelperWindowsVisible));
            set
            {
                var existingValue = IsHelperWindowsVisible;
                EditorPrefs.SetBool(nameof(IsHelperWindowsVisible), value);
                if (existingValue != value) OnInspectorVisibilityChanged?.Invoke(value);
            }
        }

        public Level.LevelEditor? LevelEditor { get; set; }

        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnInspectorVisibilityChanged;
    }
}