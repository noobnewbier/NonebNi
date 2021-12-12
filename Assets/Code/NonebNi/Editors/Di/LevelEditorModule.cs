using System;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editors.Level;
using NonebNi.Editors.Level.Entities;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Maps;
using NonebNi.Editors.Level.Settings;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Di
{
    public class LevelEditorModule
    {
        private readonly LevelData _levelData;
        private readonly LevelDataSource _levelDataSource;
        private readonly Map _map;
        private readonly Scene _scene;

        public WorldConfigData GetWorldConfigData => _levelData.WorldConfig;

        public IReadOnlyMap GetReadOnlyMap => _map;

        public LevelEditorModule(LevelDataSource levelDataSource, Scene scene)
        {
            _levelDataSource = levelDataSource;
            if (!_levelDataSource.IsValid)
                throw new ArgumentException(
                    $"{nameof(levelDataSource)} must be valid(${nameof(levelDataSource.IsValid)} should be true, otherwise no backing data can be created)"
                );

            _levelData = _levelDataSource.CreateData()!;
            _map = _levelData.Map;
            _scene = scene;
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            new CoordinateAndPositionService(_levelData.WorldConfig);

        public LevelEditorModel GetLevelEditorDataModel() => new LevelEditorModel(_levelData, _map, _levelDataSource);

        public MapSyncService GetMapSyncService(EditorEntityPositioningService editorEntityPositioningService) =>
            new MapSyncService(editorEntityPositioningService, _scene, _map);

        public EntitiesPlacer GetEntitiesPlacer(MapSyncService mapSyncService,
                                                EditorEntityPositioningService editorEntityPositioningService,
                                                ErrorChecker errorChecker) =>
            new EntitiesPlacer(mapSyncService, editorEntityPositioningService, errorChecker);

        public EditorEntityPositioningService GetEditorEntityPositioningService(
            CoordinateAndPositionService coordinateAndPositionService) =>
            new EditorEntityPositioningService(coordinateAndPositionService, _map);

        public ErrorChecker GetErrorChecker(EditorEntityPositioningService editorEntityPositioningService) =>
            new ErrorChecker(editorEntityPositioningService, _scene);

        public LevelSavingService GetLevelSavingService() =>
            new LevelSavingService(_levelDataSource, _levelData);

        public LevelDataSyncer GetLevelDataSyncer(LevelSavingService levelSavingService) =>
            new LevelDataSyncer(levelSavingService, _scene);
    }
}