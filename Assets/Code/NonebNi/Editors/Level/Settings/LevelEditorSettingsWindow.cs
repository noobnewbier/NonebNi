using NonebNi.Editors.Di;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Level.Maps;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.Editors.Level.Settings
{
    public class LevelEditorSettingsWindow : EditorWindow
    {
        private LevelEditorModel _dataModel = null!;
        private LevelSavingService _levelSavingService = null!;
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
                typeof(WorldConfigSource),
                false
            ) as WorldConfigSource;

            if (GUILayout.Button("Refresh")) _mapSyncService.Sync();
            if (GUILayout.Button("Save")) _levelSavingService.Save();


            if (GUILayout.Button("Done"))
                Close();
        }

        public static LevelEditorSettingsWindow Init(ILevelEditorComponent component)
        {
            var toReturn = CreateInstance<LevelEditorSettingsWindow>();
            toReturn._dataModel = component.LevelEditorModel;
            toReturn._mapSyncService = component.MapSyncService;
            toReturn._levelSavingService = component.LevelSavingService;

            return toReturn;
        }
    }
}