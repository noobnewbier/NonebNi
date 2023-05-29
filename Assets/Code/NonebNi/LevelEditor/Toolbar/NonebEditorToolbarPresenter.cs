using System.Diagnostics.CodeAnalysis;
using NonebNi.LevelEditor.Level;
using NonebNi.LevelEditor.Level.Settings;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Toolbar
{
    public class NonebEditorToolbarPresenter
    {
        private readonly NonebEditorModel _model;
        private readonly NonebEditorToolbarView _view;
        private LevelEditorSettingsWindow? _settingsWindowCache;

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
                _settingsWindowCache = null;

                return false;
            }

            if (_settingsWindowCache == null)
            {
                _settingsWindowCache = _model.LevelEditor.CreateSettingsWindow();
            }

            settingsWindow = _settingsWindowCache;
            return settingsWindow != null;
        }
    }
}