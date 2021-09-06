using NonebNi.Core.Maps;
using NonebNi.Editor.Di;

namespace NonebNi.Editor.Level.Maps
{
    public class MapPresenter
    {
        private readonly MapView _mapView;
        private readonly NonebEditorModel _nonebEditorModel;

        public bool IsDrawingGizmos => _nonebEditorModel.IsGizmosVisible;
        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;

        public MapPresenter(MapView mapView, ILevelEditorComponent component)
        {
            _mapView = mapView;

            _nonebEditorModel = component.NonebEditorModel;

            var levelEditorModel = component.LevelEditorModel;
            levelEditorModel.OnMapChanged += OnMapChanged;
        }

        private void OnMapChanged(Map map)
        {
            _mapView.Map = map;
        }
    }
}