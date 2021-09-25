using NonebNi.Core.Maps;
using NonebNi.Editor.Di;

namespace NonebNi.Editor.Level.Maps
{
    public class MapPresenter
    {
        private readonly LevelEditorModel _levelEditorModel;
        private readonly MapView _mapView;
        private readonly NonebEditorModel _nonebEditorModel;

        public bool IsDrawingGizmos => _nonebEditorModel.IsGizmosVisible;
        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;
        public Map Map => _levelEditorModel.Map;

        public MapPresenter(MapView mapView, ILevelEditorComponent component)
        {
            _mapView = mapView;

            _nonebEditorModel = component.NonebEditorModel;
            _levelEditorModel = component.LevelEditorModel;
        }
    }
}