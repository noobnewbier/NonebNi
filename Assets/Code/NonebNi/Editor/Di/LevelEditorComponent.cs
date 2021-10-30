using System;
using NonebNi.Core.Level;
using NonebNi.Editor.Level;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Error;
using NonebNi.Editor.Level.Inspector;
using NonebNi.Editor.Level.Maps;

namespace NonebNi.Editor.Di
{
    public interface ILevelEditorComponent
    {
        LevelEditorModel LevelEditorModel { get; }
        MapSyncService MapSyncService { get; }
        CoordinateAndPositionService CoordinateAndPositionService { get; }
        NonebEditorModel NonebEditorModel { get; }
        EntitiesPlacer EntitiesPlacer { get; }
        ErrorChecker ErrorChecker { get; }

        GridView GridView { get; }
        TileInspectorView TileInspectorView { get; }
        GridPresenter CreateMapPresenter(GridView view);
        TileInspectorPresenter CreateTileInspectorPresenter(TileInspectorView view);
        ErrorOverviewPresenter CreateErrorOverviewPresenter(ErrorOverviewView view);
    }

    public class LevelEditorComponent : ILevelEditorComponent
    {
        private readonly Lazy<CoordinateAndPositionService> _lazyCoordinateAndPositionService;
        private readonly Lazy<LevelEditorModel> _lazyEditorDataModel;
        private readonly Lazy<EntitiesPlacer> _lazyEntitiesPlacer;
        private readonly Lazy<ErrorChecker> _lazyErrorChecker;
        private readonly Lazy<ErrorOverviewView> _lazyErrorOverviewView;
        private readonly Lazy<MapSyncService> _lazyMapEditingService;
        private readonly Lazy<GridView> _lazyMapView;
        private readonly Lazy<TileInspectorView> _lazyTileInspectorView;
        public ErrorOverviewView ErrorOverviewView => _lazyErrorOverviewView.Value;

        public LevelEditorComponent(LevelEditorModule module, INonebEditorComponent nonebEditorComponent)
        {
            _lazyEditorDataModel = new Lazy<LevelEditorModel>(module.GetLevelEditorDataModel);
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(module.GetCoordinateAndPositionService);
            _lazyMapEditingService = new Lazy<MapSyncService>(
                () => module.GetMapEditingService(CoordinateAndPositionService)
            );
            _lazyEntitiesPlacer = new Lazy<EntitiesPlacer>(
                () => module.GetEntitiesPlacer(MapSyncService, CoordinateAndPositionService, ErrorChecker)
            );
            _lazyErrorChecker = new Lazy<ErrorChecker>(() => module.GetErrorChecker(CoordinateAndPositionService));

            _lazyMapView = new Lazy<GridView>(
                () => new GridView(this, CoordinateAndPositionService, module.GetWorldConfigData)
            );
            _lazyTileInspectorView = new Lazy<TileInspectorView>(
                () => new TileInspectorView(this, module.GetWorldConfigData, module.GetReadOnlyMap)
            );
            _lazyErrorOverviewView = new Lazy<ErrorOverviewView>(() => new ErrorOverviewView(this));

            NonebEditorModel = nonebEditorComponent.NonebEditorModel;
        }

        //todo: pass in a setting class instead
        public NonebEditorModel NonebEditorModel { get; }

        public EntitiesPlacer EntitiesPlacer => _lazyEntitiesPlacer.Value;
        public ErrorChecker ErrorChecker => _lazyErrorChecker.Value;
        public LevelEditorModel LevelEditorModel => _lazyEditorDataModel.Value;
        public MapSyncService MapSyncService => _lazyMapEditingService.Value;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;

        public GridView GridView => _lazyMapView.Value;
        public TileInspectorView TileInspectorView => _lazyTileInspectorView.Value;

        public GridPresenter CreateMapPresenter(GridView view) =>
            new GridPresenter(view, LevelEditorModel, NonebEditorModel);

        public TileInspectorPresenter CreateTileInspectorPresenter(TileInspectorView view) =>
            new TileInspectorPresenter(view, CoordinateAndPositionService, NonebEditorModel);

        public ErrorOverviewPresenter CreateErrorOverviewPresenter(ErrorOverviewView view) =>
            new ErrorOverviewPresenter(view, ErrorChecker);
    }
}