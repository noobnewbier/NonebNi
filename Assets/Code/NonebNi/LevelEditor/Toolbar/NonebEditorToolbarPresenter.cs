using System.Diagnostics.CodeAnalysis;
using NonebNi.LevelEditor.Level.Data;
using NonebNi.LevelEditor.Level.Settings;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Toolbar
{
    public class NonebEditorToolbarPresenter
    {
        private readonly NonebEditorModel _model;
        private readonly NonebEditorToolbarView _view;

        public NonebEditorToolbarPresenter(NonebEditorToolbarView view, NonebEditorModel nonebEditorModel)
        {
            _view = view;
            _model = nonebEditorModel;
        }

        public bool IsGridVisible => _model.IsGridVisible;
        public bool IsHelperWindowsVisible => _model.IsHelperWindowsVisible;

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

        public bool TryGetSettingsWindow([NotNullWhen(true)] out LevelEditorSettingsWindow? settingsWindow)
        {
            if (_model.LevelEditor == null)
            {
                settingsWindow = default;
                return false;
            }

            settingsWindow = _model.LevelEditor.GetSettingsWindow();
            return _model.LevelEditor.GetSettingsWindow();
        }
    }
}