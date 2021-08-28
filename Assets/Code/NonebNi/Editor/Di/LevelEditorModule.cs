using NonebNi.Core.Entity;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;

namespace NonebNi.Editor.Di
{
    public class LevelEditorModule
    {
        private readonly LevelData _levelData;

        public CoordinateAndPositionService CoordinateAndPositionService =>
            new CoordinateAndPositionService(_levelData.WorldConfig);

        public LevelEditorModule(LevelData levelData)
        {
            _levelData = levelData;
        }

        public LevelEditorDataModel GetLevelEditorDataModel() => new LevelEditorDataModel(_levelData);
        public MapGenerationService GetMapGenerationService() => new MapGenerationService();

        public EntityService GetEntityService(CoordinateAndPositionService coordinateAndPositionService) =>
            new EntityService(coordinateAndPositionService);
    }
}