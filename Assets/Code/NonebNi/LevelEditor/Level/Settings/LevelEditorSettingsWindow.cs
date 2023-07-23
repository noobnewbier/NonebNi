using NonebNi.LevelEditor.Level.Maps;
using NonebNi.Terrain;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.LevelEditor.Level.Settings
{
    public class LevelEditorSettingsWindow : EditorWindow
    {
        private LevelEditorModel _dataModel = null!;
        private EditorEntitySyncService _editorEntitySyncService = null!;
        private LevelSavingService _levelSavingService = null!;

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
                typeof(TerrainConfigSource),
                false
            ) as TerrainConfigSource;

            if (GUILayout.Button("Refresh")) _editorEntitySyncService.Sync();
            if (GUILayout.Button("Save")) _levelSavingService.Save();


            if (GUILayout.Button("Done"))
                Close();
        }

        public static LevelEditorSettingsWindow Init(
            LevelEditorModel editorModel,
            EditorEntitySyncService editorEntitySyncService,
            LevelSavingService levelSavingService)
        {
            var toReturn = CreateInstance<LevelEditorSettingsWindow>();
            toReturn._dataModel = editorModel;
            toReturn._editorEntitySyncService = editorEntitySyncService;
            toReturn._levelSavingService = levelSavingService;

            return toReturn;
        }
    }
}