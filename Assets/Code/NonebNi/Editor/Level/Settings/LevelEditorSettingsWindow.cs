using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Maps;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Settings
{
    public class LevelEditorSettingsWindow : EditorWindow
    {
        private LevelEditorModel _dataModel = null!;
        private MapEditingService _mapEditingService = null!;

        private void OnGUI()
        {
            GUILayout.Label("LevelSettings", EditorStyles.boldLabel);

            var levelDataSource = _dataModel.LevelDataSource;
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(
                        nameof(levelDataSource),
                        levelDataSource,
                        typeof(LevelDataSource),
                        false
                    );
                }
            }

            levelDataSource.MapConfig = EditorGUILayout.ObjectField(
                nameof(levelDataSource.MapConfig),
                levelDataSource.MapConfig,
                typeof(MapConfigScriptable),
                false
            ) as MapConfigScriptable;

            levelDataSource.WorldConfig = EditorGUILayout.ObjectField(
                nameof(levelDataSource.WorldConfig),
                levelDataSource.WorldConfig,
                typeof(WorldConfigScriptable),
                false
            ) as WorldConfigScriptable;

            if (GUILayout.Button("Refresh")) _mapEditingService.RefreshMap();


            if (GUILayout.Button("Done"))
                Close();
        }

        public static LevelEditorSettingsWindow Init(LevelEditorComponent component)
        {
            var toReturn = CreateInstance<LevelEditorSettingsWindow>();
            toReturn._dataModel = component.LevelEditorModel;
            toReturn._mapEditingService = component.MapEditingService;

            return toReturn;
        }
    }
}