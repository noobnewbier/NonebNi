using NonebNi.LevelEditor.Level.Maps;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.LevelEditor.Level.Settings
{
    public class LevelEditorSettingsWindow : EditorWindow
    {
        private LevelEditorModel _dataModel = null!;
        private LevelSavingService _levelSavingService = null!;
        private MapSyncService _mapSyncService = null!;

        private void OnGUI()
        {
            GUILayout.Label("LevelSettings", EditorStyles.boldLabel);

            var levelDataSource = _dataModel.EditorLevelDataSource;
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(
                        nameof(levelDataSource),
                        levelDataSource,
                        typeof(EditorLevelDataSource),
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

        public static LevelEditorSettingsWindow Init(LevelEditorModel editorModel,
            MapSyncService mapSyncService,
            LevelSavingService levelSavingService)
        {
            var toReturn = CreateInstance<LevelEditorSettingsWindow>();
            toReturn._dataModel = editorModel;
            toReturn._mapSyncService = mapSyncService;
            toReturn._levelSavingService = levelSavingService;

            return toReturn;
        }
    }
}