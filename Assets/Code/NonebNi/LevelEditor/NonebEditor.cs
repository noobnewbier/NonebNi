using System;
using NonebNi.LevelEditor.Common.Events;
using NonebNi.LevelEditor.Di;
using NonebNi.LevelEditor.Level;
using NonebNi.LevelEditor.Toolbar;
using StrongInject;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor
{
    public class NonebEditor : IDisposable
    {
        private readonly NonebEditorModel _editorModel;
        private readonly NonebEditorToolbarView _toolbar;

        private Owned<Level.LevelEditor>? _levelEditor;

        public NonebEditor(NonebEditorToolbarView toolbar, NonebEditorModel editorModel)
        {
            _toolbar = toolbar;
            _editorModel = editorModel;

            TryInitLevelEditor();

            SceneView.duringSceneGui += OnSceneGUI;
            LevelDataSourceChangedListener.OnLevelDataSourceChanged += TryInitLevelEditorAfterOneFrame;
            EditorSceneManager.activeSceneChangedInEditMode += OnActiveSceneChanged;
        }

        public void Dispose()
        {
            CleanUp();
        }

        ~NonebEditor()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            _levelEditor?.Dispose();

            SceneView.duringSceneGui -= OnSceneGUI;
            LevelDataSourceChangedListener.OnLevelDataSourceChanged -= TryInitLevelEditorAfterOneFrame;
            EditorSceneManager.activeSceneChangedInEditMode -= OnActiveSceneChanged;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            _toolbar.DrawSceneToolbar(sceneView);
        }

        private void OnActiveSceneChanged(Scene _, Scene __)
        {
            TryInitLevelEditor();
        }

        private void TryInitLevelEditor()
        {
            _levelEditor?.Dispose();
            _levelEditor = null;
            var dataSource = EditorLevelDataSource.FindLevelDataSourceForActiveScene();
            if (dataSource == null || !dataSource.IsValid) return;

            _levelEditor = new LevelEditorContainer(dataSource, SceneManager.GetActiveScene(), _editorModel).Resolve();
            _editorModel.LevelEditor = _levelEditor.Value;
        }

        private void TryInitLevelEditorAfterOneFrame()
        {
            EditorApplication.delayCall += () =>
            {
                TryInitLevelEditor();
                SceneView.RepaintAll();
            };
        }
    }
}