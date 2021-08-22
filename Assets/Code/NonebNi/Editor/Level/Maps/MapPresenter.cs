using NonebNi.Editor.Di;

namespace NonebNi.Editor.Level.Maps
{
    public class MapPresenter
    {
        private readonly MapView _mapView;

        public MapPresenter(MapView mapView, ILevelEditorComponent component)
        {
            _mapView = mapView;

            LevelEditorDataModel dataModel = component.LevelEditorDataModel;
            dataModel.OnGridVisibilityChanged += OnGridVisibilityChanged;
            dataModel.OnGizmosVisibilityChanged += OnGizmosVisibilityChanged;
        }

        private void OnGizmosVisibilityChanged(bool isVisible)
        {
            _mapView.IsDrawingGizmos = isVisible;
        }

        private void OnGridVisibilityChanged(bool isVisible)
        {
            _mapView.IsDrawingGrid = isVisible;
        }
    }
}