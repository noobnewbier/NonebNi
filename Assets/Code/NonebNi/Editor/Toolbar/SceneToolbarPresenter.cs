using NonebNi.Editor.Di;

namespace NonebNi.Editor.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly NonebEditorModel _model;
        private readonly SceneToolbarView _view;

        public bool IsGridVisible => _model.IsGridVisible;
        public bool IsGizmosVisible => _model.IsGizmosVisible;

        public SceneToolbarPresenter(SceneToolbarView view, INonebEditorComponent component)
        {
            _view = view;
            _model = component.NonebEditorModel;
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