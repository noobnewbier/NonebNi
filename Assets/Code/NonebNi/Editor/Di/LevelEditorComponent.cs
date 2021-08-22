using System;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;

namespace NonebNi.Editor.Di
{
    public interface ILevelEditorComponent
    {
        LevelEditorDataModel LevelEditorDataModel { get; }
        MapGenerationService MapGenerationService { get; }
        CoordinateAndPositionService CoordinateAndPositionService { get; }
    }

    public class LevelEditorComponent : ILevelEditorComponent
    {
        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;
        private readonly Lazy<LevelEditorDataModel> _lazyEditorDataModel;
        private readonly Lazy<MapGenerationService> _lazyMapGenerationService;

        public LevelEditorComponent(LevelEditorModule module)
        {
            _lazyEditorDataModel = new Lazy<LevelEditorDataModel>(() => module.LevelEditorDataModel);
            _lazyMapGenerationService = new Lazy<MapGenerationService>(() => module.MapGenerationService);
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(() => module.CoordinateAndPositionService);
        }

        public LevelEditorDataModel LevelEditorDataModel => _lazyEditorDataModel.Value;
        public MapGenerationService MapGenerationService => _lazyMapGenerationService.Value;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
    }
}