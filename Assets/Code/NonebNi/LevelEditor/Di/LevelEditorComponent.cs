using System;
using NonebNi.Core.Coordinates;
using NonebNi.LevelEditor.Level;
using NonebNi.LevelEditor.Level.Entities;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Inspector;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Settings;

namespace NonebNi.LevelEditor.Di
{
    public interface ILevelEditorComponent
    {
        LevelEditorModel LevelEditorModel { get; }
        MapSyncService MapSyncService { get; }
        CoordinateAndPositionService CoordinateAndPositionService { get; }
        EditorEntityPositioningService EditorEntityPositioningService { get; }
        LevelSavingService LevelSavingService { get; }
        NonebEditorModel NonebEditorModel { get; }
        EntitiesPlacer EntitiesPlacer { get; }
        ErrorChecker ErrorChecker { get; }
        LevelDataSyncer LevelDataSyncer { get; }
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
        private readonly Lazy<EditorEntityPositioningService> _lazyEditorEntityPositioningService;
        private readonly Lazy<EntitiesPlacer> _lazyEntitiesPlacer;
        private readonly Lazy<ErrorChecker> _lazyErrorChecker;
        private readonly Lazy<ErrorOverviewView> _lazyErrorOverviewView;
        private readonly Lazy<LevelDataSyncer> _lazyLevelDataSyncer;
        private readonly Lazy<LevelSavingService> _lazyLevelSavingService;
        private readonly Lazy<MapSyncService> _lazyMapEditingService;
        private readonly Lazy<GridView> _lazyMapView;
        private readonly Lazy<TileInspectorView> _lazyTileInspectorView;
        public ErrorOverviewView ErrorOverviewView => _lazyErrorOverviewView.Value;

        public LevelEditorComponent(LevelEditorModule module, INonebEditorComponent nonebEditorComponent)
        {
            _lazyEditorDataModel = new Lazy<LevelEditorModel>(module.GetLevelEditorDataModel);
            _lazyCoordinateAndPositionService =
                new Lazy<CoordinateAndPositionService>(module.GetCoordinateAndPositionService);
            _lazyEditorEntityPositioningService = new Lazy<EditorEntityPositioningService>(
                () => module.GetEditorEntityPositioningService(CoordinateAndPositionService)
            );
            _lazyLevelSavingService = new Lazy<LevelSavingService>(module.GetLevelSavingService);
            _lazyMapEditingService =
                new Lazy<MapSyncService>(() => module.GetMapSyncService(EditorEntityPositioningService));
            _lazyEntitiesPlacer = new Lazy<EntitiesPlacer>(
                () => module.GetEntitiesPlacer(MapSyncService, EditorEntityPositioningService, ErrorChecker)
            );
            _lazyErrorChecker = new Lazy<ErrorChecker>(() => module.GetErrorChecker(EditorEntityPositioningService));
            _lazyLevelDataSyncer =
                new Lazy<LevelDataSyncer>(() => module.GetLevelDataSyncer(LevelSavingService, MapSyncService));

            _lazyMapView = new Lazy<GridView>(
                () => new GridView(this, CoordinateAndPositionService, module.GetWorldConfigData)
            );
            _lazyTileInspectorView = new Lazy<TileInspectorView>(
                () => new TileInspectorView(this, module.GetWorldConfigData, module.GetEditorEditorMap)
            );
            _lazyErrorOverviewView = new Lazy<ErrorOverviewView>(() => new ErrorOverviewView(this));

            NonebEditorModel = nonebEditorComponent.NonebEditorModel;
        }

        //todo: pass in a setting class instead
        public NonebEditorModel NonebEditorModel { get; }
        public LevelSavingService LevelSavingService => _lazyLevelSavingService.Value;
        public EntitiesPlacer EntitiesPlacer => _lazyEntitiesPlacer.Value;
        public ErrorChecker ErrorChecker => _lazyErrorChecker.Value;
        public LevelDataSyncer LevelDataSyncer => _lazyLevelDataSyncer.Value;
        public LevelEditorModel LevelEditorModel => _lazyEditorDataModel.Value;
        public MapSyncService MapSyncService => _lazyMapEditingService.Value;
        public CoordinateAndPositionService CoordinateAndPositionService => _lazyCoordinateAndPositionService.Value;
        public EditorEntityPositioningService EditorEntityPositioningService => _lazyEditorEntityPositioningService.Value;

        public GridView GridView => _lazyMapView.Value;
        public TileInspectorView TileInspectorView => _lazyTileInspectorView.Value;

        public GridPresenter CreateMapPresenter(GridView view) =>
            new GridPresenter(view, LevelEditorModel, NonebEditorModel);

        public TileInspectorPresenter CreateTileInspectorPresenter(TileInspectorView view) =>
            new TileInspectorPresenter(view, CoordinateAndPositionService, NonebEditorModel);

        public ErrorOverviewPresenter CreateErrorOverviewPresenter(ErrorOverviewView view) =>
            new ErrorOverviewPresenter(view, ErrorChecker, NonebEditorModel);
    }
}