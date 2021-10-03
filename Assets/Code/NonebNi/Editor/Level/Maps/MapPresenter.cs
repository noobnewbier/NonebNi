using NonebNi.Core.Maps;

namespace NonebNi.Editor.Level.Maps
{
    public class MapPresenter
    {
        private readonly LevelEditorModel _levelEditorModel;
        private readonly MapView _mapView;
        private readonly NonebEditorModel _nonebEditorModel;

        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;
        public IReadOnlyMap Map => _levelEditorModel.Map;

        public MapPresenter(MapView mapView, LevelEditorModel levelEditorModel, NonebEditorModel nonebEditorModel)
        {
            _mapView = mapView;
            _levelEditorModel = levelEditorModel;
            _nonebEditorModel = nonebEditorModel;
        }
    }
}