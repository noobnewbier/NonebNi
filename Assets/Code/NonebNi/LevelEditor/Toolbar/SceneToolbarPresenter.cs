using NonebNi.LevelEditor.Di;
using NonebNi.LevelEditor.Level.Data;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Toolbar
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
            EditorLevelDataSource.CreateSource(SceneManager.GetActiveScene());
        }
    }
}