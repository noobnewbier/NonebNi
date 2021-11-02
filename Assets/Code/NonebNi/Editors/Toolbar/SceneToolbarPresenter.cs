using NonebNi.Editors.Di;
using NonebNi.Editors.Level;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly NonebEditorModel _model;
        private readonly NonebEditorToolbarView _view;
        public readonly NonebEditor NonebEditor;

        public bool IsGridVisible => _model.IsGridVisible;
        public bool IsHelperWindowsVisible => _model.IsHelperWindowsVisible;

        public SceneToolbarPresenter(NonebEditorToolbarView view, INonebEditorComponent component)
        {
            _view = view;
            NonebEditor = component.NonebEditor;
            _model = component.NonebEditorModel;
        }

        public void OnToggleGridVisibility()
        {
            _model.IsGridVisible = !_model.IsGridVisible;
        }

        public void OnToggleHelperWindowsVisibility()
        {
            _model.IsHelperWindowsVisible = !_model.IsHelperWindowsVisible;
        }

        public void ConvertActiveSceneToLevel()
        {
            LevelDataSource.CreateSource(SceneManager.GetActiveScene());
        }
    }
}