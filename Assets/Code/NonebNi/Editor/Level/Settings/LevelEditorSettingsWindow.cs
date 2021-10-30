using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Maps;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.Editor.Level.Settings
{
    public class LevelEditorSettingsWindow : EditorWindow
    {
        private LevelEditorModel _dataModel = null!;
        private MapSyncService _mapSyncService = null!;

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

            //todo:
            GUILayout.Label("NOT IMPL: MapDimensions Changes", NonebGUIStyle.Error);

            levelDataSource.WorldConfig = EditorGUILayout.ObjectField(
                nameof(levelDataSource.WorldConfig),
                levelDataSource.WorldConfig,
                typeof(WorldConfigScriptable),
                false
            ) as WorldConfigScriptable;

            if (GUILayout.Button("Refresh")) _mapSyncService.Sync();


            if (GUILayout.Button("Done"))
                Close();
        }

        public static LevelEditorSettingsWindow Init(ILevelEditorComponent component)
        {
            var toReturn = CreateInstance<LevelEditorSettingsWindow>();
            toReturn._dataModel = component.LevelEditorModel;
            toReturn._mapSyncService = component.MapSyncService;

            return toReturn;
        }
    }
}