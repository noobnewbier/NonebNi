using System;
using NonebNi.LevelEditor.Common.Events;
using NonebNi.LevelEditor.Di;
using NonebNi.LevelEditor.Level.Data;
using NonebNi.LevelEditor.Toolbar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor
{
    public class NonebEditor : IDisposable
    {
        private readonly NonebEditorComponent _component;
        private readonly NonebEditorModel _model;

        private readonly NonebEditorToolbarView _toolbar;

        public Level.LevelEditor? LevelEditor { get; private set; }

        public NonebEditor()
        {
            _component = new NonebEditorComponent(new NonebEditorModule(this));
            _model = _component.NonebEditorModel;

            _toolbar = new NonebEditorToolbarView(_component);

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
            LevelEditor?.Dispose();

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
            LevelEditor?.Dispose();
            LevelEditor = null;
            var dataSource = EditorLevelDataSource.FindLevelDataSourceForActiveScene();
            if (dataSource != null && dataSource.IsValid)
                LevelEditor = new Level.LevelEditor(SceneManager.GetActiveScene(), dataSource, _component);
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