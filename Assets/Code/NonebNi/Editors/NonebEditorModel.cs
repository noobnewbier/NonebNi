using System;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Inspector;
using UnityEditor;

namespace NonebNi.Editors
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

        public event Action<bool>? OnGridVisibilityChanged;
        public event Action<bool>? OnInspectorVisibilityChanged;
    }
}