using NonebNi.Core.Level;
using NonebNi.Editor.ServiceLocator;

namespace NonebNi.Editor.Level.Map
{
    public class MapPresenter
    {
        private readonly LevelEditorDataModel _dataModel;
        private readonly MapView _mapView;

        public MapPresenter(MapView mapView)
        {
            _mapView = mapView;
            _dataModel = NonebEditorServiceLocator.Instance.LevelEditorDataModel;

            _dataModel.OnGridVisibilityChanged += OnGridVisibilityChanged;
            _dataModel.OnGizmosVisibilityChanged += OnGizmosVisibilityChanged;
            _dataModel.OnLevelDataChanged += OnLevelDataChanged;
        }

        private void OnLevelDataChanged(LevelData data)
        {
            _mapView.SetUpData(data.Map, data.WorldConfig);
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