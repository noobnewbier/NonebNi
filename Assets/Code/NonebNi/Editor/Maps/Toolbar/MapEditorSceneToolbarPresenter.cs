namespace NonebNi.Editor.Maps.Toolbar
{
    public class MapEditorSceneToolbarPresenter
    {
        private readonly MapEditor _mapEditor;
        private readonly MapEditorSceneToolbarView _view;

        public MapEditorSceneToolbarPresenter(MapEditorSceneToolbarView view, MapEditor mapEditor)
        {
            _view = view;
            _mapEditor = mapEditor;
        }

        public void OnToggleGridVisibility()
        {
            _mapEditor.IsDrawGridOverlay = !_mapEditor.IsDrawGridOverlay;
        }

        public void OnToggleGizmosVisibility()
        {
            _mapEditor.IsDrawGizmosOverlay = !_mapEditor.IsDrawGizmosOverlay;
        }
    }
}