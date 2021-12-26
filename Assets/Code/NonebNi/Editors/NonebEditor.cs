using System;
using System.Linq;
using NonebNi.Editors.Common.Events;
using NonebNi.Editors.Di;
using NonebNi.Editors.Level;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Toolbar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors
{
    public class NonebEditor : IDisposable
    {
        private readonly NonebEditorComponent _component;
        private readonly NonebEditorModel _model;

        private readonly NonebEditorToolbarView _toolbar;

        public LevelEditor? LevelEditor { get; private set; }

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

        private static EditorLevelDataSource? FindLevelDataSourceForActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            var allLevelDatas = AssetDatabase.FindAssets($"t:{nameof(EditorLevelDataSource)}")
                                             .Select(AssetDatabase.GUIDToAssetPath)
                                             .Select(AssetDatabase.LoadAssetAtPath<EditorLevelDataSource>);
            var matchingData = allLevelDatas.FirstOrDefault(s => s.SceneName == activeScene.name);

            return matchingData;
        }

        private void OnActiveSceneChanged(Scene _, Scene __)
        {
            TryInitLevelEditor();
        }

        private void TryInitLevelEditor()
        {
            LevelEditor?.Dispose();
            LevelEditor = null;
            var dataSource = FindLevelDataSourceForActiveScene();
            if (dataSource != null && dataSource.IsValid)
                LevelEditor = new LevelEditor(SceneManager.GetActiveScene(), dataSource, _component);
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