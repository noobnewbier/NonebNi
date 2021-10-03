using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Editor.Level;
using NonebNi.Editor.Level.Inspector;
using NonebNi.Editor.Level.Maps;

namespace NonebNi.Editor.Di
{
    public interface ILevelEditorComponent
    {
        LevelEditorModel LevelEditorModel { get; }
        MapEditingService MapEditingService { get; }
        CoordinateAndPositionService CoordinateAndPositionService { get; }
        EntityService EntityService { get; }
        NonebEditorModel NonebEditorModel { get; }

        GridView GridView { get; }
        TileInspectorView TileInspectorView { get; }
        GridPresenter CreateMapPresenter(GridView view);
        TileInspectorPresenter CreateTileInspectorPresenter(TileInspectorView view);
    }

    public class LevelEditorComponent : ILevelEditorComponent
    {
        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;
        private readonly Lazy<LevelEditorModel> _lazyEditorDataModel;
        private readonly Lazy<EntityService> _lazyEntityService;
        private readonly Lazy<MapEditingService> _lazyMapGenerationService;
        private readonly Lazy<GridView> _lazyMapView;
        private readonly Lazy<TileInspectorView> _lazyTileInspectorView;

        public LevelEditorComponent(LevelEditorModule module, INonebEditorComponent nonebEditorComponent)
        {
            _lazyEditorDataModel = new Lazy<LevelEditorModel>(module.GetLevelEditorDataModel);
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(module.GetCoordinateAndPositionService);
            _lazyMapGenerationService = new Lazy<MapEditingService>(
                () => module.GetMapEditingService(_lazyCoordinateAndPositionService.Value)
            );
            _lazyEntityService =
                new Lazy<EntityService>(() => module.GetEntityService(_lazyCoordinateAndPositionService.Value));
            _lazyMapView = new Lazy<GridView>(
                () => new GridView(this, CoordinateAndPositionService, module.GetWorldConfigData)
            );
            _lazyTileInspectorView = new Lazy<TileInspectorView>(
                () => new TileInspectorView(this, module.GetWorldConfigData, module.GetReadOnlyMap)
            );

            NonebEditorModel = nonebEditorComponent.NonebEditorModel;
        }

        //todo: pass in a setting class instead
        public NonebEditorModel NonebEditorModel { get; }
        public LevelEditorModel LevelEditorModel => _lazyEditorDataModel.Value;
        public MapEditingService MapEditingService => _lazyMapGenerationService.Value;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
        public EntityService EntityService => _lazyEntityService.Value;

        public GridView GridView => _lazyMapView.Value;
        public TileInspectorView TileInspectorView => _lazyTileInspectorView.Value;

        public GridPresenter CreateMapPresenter(GridView view) =>
            new GridPresenter(view, LevelEditorModel, NonebEditorModel);

        public TileInspectorPresenter CreateTileInspectorPresenter(TileInspectorView view) =>
            new TileInspectorPresenter(view, CoordinateAndPositionService, NonebEditorModel);
    }
}