﻿using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Toolbar
{
    public class SceneToolbarPresenter
    {
        private readonly NonebEditorModel _model;
        private readonly NonebEditorToolbarView _view;
        public readonly NonebEditor NonebEditor;

        public bool IsGridVisible => _model.IsGridVisible;
        public bool IsGizmosVisible => _model.IsInspectorVisible;

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

        public void OnToggleGizmosVisibility()
        {
            _model.IsInspectorVisible = !_model.IsInspectorVisible;
        }

        public void ConvertActiveSceneToLevel()
        {
            LevelDataSource.CreateSource(SceneManager.GetActiveScene());
        }
    }
}