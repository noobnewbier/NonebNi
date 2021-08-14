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

            _dataModel.OnGridVisibilityChanged += OnVisibilityChanged;
            _dataModel.OnLevelDataChanged += OnLevelDataChanged;
        }

        private void OnLevelDataChanged(LevelData data)
        {
            if (_dataModel.IsGridVisible) _mapView.StartDrawGridWithData(data.Map, data.WorldConfig);
        }

        private void OnVisibilityChanged(bool isVisible)
        {
            if (isVisible && _dataModel.CurrentLevelData != null)
                _mapView.StartDrawGridWithData(_dataModel.CurrentLevelData.Map, _dataModel.CurrentLevelData.WorldConfig);
            else _mapView.StopDrawing();
        }
    }
}