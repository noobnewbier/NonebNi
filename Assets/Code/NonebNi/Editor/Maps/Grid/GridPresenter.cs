using NonebNi.Core.Level;
using NonebNi.Editor.ServiceLocator;

namespace NonebNi.Editor.Maps.Grid
{
    public class GridPresenter
    {
        private readonly LevelEditorDataModel _dataModel;
        private readonly GridView _gridView;

        public GridPresenter(GridView gridView)
        {
            _gridView = gridView;
            _dataModel = NonebEditorServiceLocator.Instance.LevelEditorDataModel;

            _dataModel.OnGridVisibilityChanged += OnVisibilityChanged;
            _dataModel.OnLevelDataChanged += OnLevelDataChanged;
        }

        private void OnLevelDataChanged(LevelData data)
        {
            if (_dataModel.IsGridVisible) _gridView.StartDrawGridWithData(data.Map, data.WorldConfig);
        }

        private void OnVisibilityChanged(bool isVisible)
        {
            if (isVisible && _dataModel.CurrentLevelData != null)
                _gridView.StartDrawGridWithData(_dataModel.CurrentLevelData.Map, _dataModel.CurrentLevelData.WorldConfig);
            else _gridView.StopDrawing();
        }
    }
}