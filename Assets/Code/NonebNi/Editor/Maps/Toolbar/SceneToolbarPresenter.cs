namespace NonebNi.Editor.Maps.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly MapEditor _mapEditor;
        private readonly SceneToolbarView _view;

        public SceneToolbarPresenter(SceneToolbarView view, MapEditor mapEditor)
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