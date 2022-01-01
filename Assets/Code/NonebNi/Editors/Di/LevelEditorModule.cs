using System;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Editors.Level;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Level.Entities;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Maps;
using NonebNi.Editors.Level.Settings;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Di
{
    public class LevelEditorModule
    {
        private readonly EditorLevelData _editorLevelData;
        private readonly EditorMap _editorMap;
        private readonly EditorLevelDataSource _levelDataSource;
        private readonly Scene _scene;

        public WorldConfigData GetWorldConfigData => _editorLevelData.WorldConfig;

        public IEditorMap GetEditorEditorMap => _editorMap;

        public LevelEditorModule(EditorLevelDataSource levelDataSource, Scene scene)
        {
            _levelDataSource = levelDataSource;
            if (!_levelDataSource.IsValid)
                throw new ArgumentException(
                    $"{nameof(levelDataSource)} must be valid(${nameof(levelDataSource.IsValid)} should be true, otherwise no backing data can be created)"
                );

            _editorLevelData = _levelDataSource.CreateData()!;
            _editorMap = _editorLevelData.Map;
            _scene = scene;
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            new CoordinateAndPositionService(_editorLevelData.WorldConfig);

        public LevelEditorModel GetLevelEditorDataModel() =>
            new LevelEditorModel(_editorLevelData, _editorMap, _levelDataSource);

        public MapSyncService GetMapSyncService(EditorEntityPositioningService editorEntityPositioningService) =>
            new MapSyncService(editorEntityPositioningService, _scene, _editorMap);

        public EntitiesPlacer GetEntitiesPlacer(MapSyncService mapSyncService,
                                                EditorEntityPositioningService editorEntityPositioningService,
                                                ErrorChecker errorChecker) =>
            new EntitiesPlacer(mapSyncService, editorEntityPositioningService, errorChecker);

        public EditorEntityPositioningService GetEditorEntityPositioningService(
            CoordinateAndPositionService coordinateAndPositionService) =>
            new EditorEntityPositioningService(coordinateAndPositionService, _editorMap);

        public ErrorChecker GetErrorChecker(EditorEntityPositioningService editorEntityPositioningService) =>
            new ErrorChecker(editorEntityPositioningService, _scene);

        public LevelSavingService GetLevelSavingService() =>
            new LevelSavingService(_levelDataSource, _editorLevelData);

        public LevelDataSyncer GetLevelDataSyncer(LevelSavingService levelSavingService) =>
            new LevelDataSyncer(levelSavingService, _scene);
    }
}