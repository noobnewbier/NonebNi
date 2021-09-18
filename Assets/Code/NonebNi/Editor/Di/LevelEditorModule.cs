using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;
using NonebNi.Editor.Level.Maps;

namespace NonebNi.Editor.Di
{
    public class LevelEditorModule
    {
        private readonly LevelData _levelData;
        private readonly Map _map;

        public LevelEditorModule(LevelData levelData, Map map)
        {
            _levelData = levelData;
            _map = map;
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            new CoordinateAndPositionService(_levelData.WorldConfig);

        public LevelEditorModel GetLevelEditorDataModel() => new LevelEditorModel(_levelData, _map);

        public MapEditingService
            GetMapGenerationService(CoordinateAndPositionService coordinateAndPositionService, Map map) =>
            new MapEditingService(coordinateAndPositionService, map);

        public EntityService GetEntityService(CoordinateAndPositionService coordinateAndPositionService) =>
            new EntityService(coordinateAndPositionService);
    }
}