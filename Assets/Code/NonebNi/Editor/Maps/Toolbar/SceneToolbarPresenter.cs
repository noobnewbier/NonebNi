namespace NonebNi.Editor.Maps.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly LevelEditor _levelEditor;
        private readonly SceneToolbarView _view;

        public SceneToolbarPresenter(SceneToolbarView view, LevelEditor levelEditor)
        {
            _view = view;
            _levelEditor = levelEditor;
        }

        public void OnToggleGridVisibility()
        {
            _levelEditor.IsDrawGridOverlay = !_levelEditor.IsDrawGridOverlay;
        }

        public void OnToggleGizmosVisibility()
        {
            _levelEditor.IsDrawGizmosOverlay = !_levelEditor.IsDrawGizmosOverlay;
        }
    }
}