using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;

namespace NonebNi.Editor.Di
{
    public class LevelEditorModule
    {
        private readonly LevelData _levelData;

        public LevelEditorModule(LevelData levelData)
        {
            _levelData = levelData;
        }

        public CoordinateAndPositionService GetCoordinateAndPositionService() =>
            new CoordinateAndPositionService(_levelData.WorldConfig);

        public LevelEditorModel GetLevelEditorDataModel() => new LevelEditorModel(_levelData);
        public MapGenerationService GetMapGenerationService() => new MapGenerationService();

        public EntityService GetEntityService(CoordinateAndPositionService coordinateAndPositionService) =>
            new EntityService(coordinateAndPositionService);
    }
}