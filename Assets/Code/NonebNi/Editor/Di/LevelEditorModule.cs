using System;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Error;
using NonebNi.Editor.Level.Maps;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Di
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

        public MapSyncService GetMapEditingService(CoordinateAndPositionService coordinateAndPositionService) =>
            new MapSyncService(coordinateAndPositionService, _scene, _map);

        public EntitiesPlacer GetEntitiesPlacer(MapSyncService mapSyncService,
                                                CoordinateAndPositionService coordinateAndPositionService,
                                                ErrorChecker errorChecker) =>
            new EntitiesPlacer(mapSyncService, coordinateAndPositionService, errorChecker);

        public ErrorChecker GetErrorChecker(CoordinateAndPositionService coordinateAndPositionService) =>
            new ErrorChecker(coordinateAndPositionService, _scene);
    }
}