using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level;

namespace NonebNi.Editor.Di
{
    public interface ILevelEditorComponent
    {
        LevelEditorModel LevelEditorModel { get; }
        MapGenerationService MapGenerationService { get; }
        CoordinateAndPositionService CoordinateAndPositionService { get; }
        EntityService EntityService { get; }
        NonebEditorModel NonebEditorModel { get; }
    }

    public class LevelEditorComponent : ILevelEditorComponent
    {
        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;
        private readonly Lazy<LevelEditorModel> _lazyEditorDataModel;
        private readonly Lazy<EntityService> _lazyEntityService;
        private readonly Lazy<MapGenerationService> _lazyMapGenerationService;

        public LevelEditorComponent(LevelEditorModule module, INonebEditorComponent nonebEditorComponent)
        {
            _lazyEditorDataModel = new Lazy<LevelEditorModel>(module.GetLevelEditorDataModel);
            _lazyMapGenerationService = new Lazy<MapGenerationService>(module.GetMapGenerationService);
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(module.GetCoordinateAndPositionService);
            _lazyEntityService =
                new Lazy<EntityService>(() => module.GetEntityService(_lazyCoordinateAndPositionService.Value));

            NonebEditorModel = nonebEditorComponent.NonebEditorModel;
        }

        //todo: pass in a setting class instead
        public NonebEditorModel NonebEditorModel { get; }
        public LevelEditorModel LevelEditorModel => _lazyEditorDataModel.Value;
        public MapGenerationService MapGenerationService => _lazyMapGenerationService.Value;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
        public EntityService EntityService => _lazyEntityService.Value;
    }
}