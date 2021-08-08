using NonebNi.Editor.ServiceLocator;

namespace NonebNi.Editor.Maps.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly LevelEditorDataModel _model;
        private readonly SceneToolbarView _view;

        public bool IsGridVisible => _model.IsGridVisible;
        public bool IsGizmosVisible => _model.IsGizmosVisible;

        public SceneToolbarPresenter(SceneToolbarView view)
        {
            _view = view;
            _model = NonebEditorServiceLocator.Instance.LevelEditorDataModel;
        }

        public void OnToggleGridVisibility()
        {
            _model.IsGridVisible = !_model.IsGridVisible;
        }

        public void OnToggleGizmosVisibility()
        {
            _model.IsGizmosVisible = !_model.IsGizmosVisible;
        }
    }
}