using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Di;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level.Settings
{
    public class SettingsWindow : EditorWindow
    {
        private NonebEditorModel _dataModel = null!;

        private void OnGUI()
        {
            GUILayout.Label("SceneSettings", EditorStyles.boldLabel);

            var levelDataSource = _dataModel.LevelDataSource;
            using (new EditorGUILayout.HorizontalScope())
            {
                _dataModel.LevelDataSource = EditorGUILayout.ObjectField(
                    nameof(levelDataSource),
                    levelDataSource,
                    typeof(LevelDataSource),
                    false
                ) as LevelDataSource;

                if (_dataModel.LevelDataSource == null)
                    if (GUILayout.Button("CreateData"))
                        _dataModel.LevelDataSource = LevelDataSource.CreateSource(SceneManager.GetActiveScene());
            }

            if (levelDataSource != null)
            {
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
            }
            else
            {
                GUILayout.Label("No Level Data");
            }


            if (GUILayout.Button("Done"))
                Close();
        }

        public static SettingsWindow Init(NonebEditorComponent component)
        {
            var toReturn = CreateInstance<SettingsWindow>();
            toReturn._dataModel = component.NonebEditorModel;

            return toReturn;
        }
    }
}