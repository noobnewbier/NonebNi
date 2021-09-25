﻿using System;
using System.Linq;
using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level;
using NonebNi.Editor.Toolbar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor
{
    [InitializeOnLoad]
    internal static class NonebEditorInitializer
    {
        static NonebEditorInitializer()
        {
            NonebEditor.Init();

            PlayModeStateListener.OnEnterEditMode += NonebEditor.Init;
            EditorSceneManager.activeSceneChangedInEditMode += NonebEditor.OnActiveSceneChanged;
        }
    }

    public class NonebEditor : IDisposable
    {
        private static NonebEditor? _instance;
        private readonly NonebEditorComponent _component;
        private readonly NonebEditorModel _model;

        private readonly NonebEditorToolbarView _toolbar;

        public LevelEditor? LevelEditor { get; private set; }

        private NonebEditor()
        {
            _instance = this;

            _component = new NonebEditorComponent(new NonebEditorModule(this));
            _model = _component.NonebEditorModel;

            _toolbar = new NonebEditorToolbarView(_component);

            TryInitLevelEditor();

            SceneView.duringSceneGui += OnSceneGUI;
            LevelDataSourceChangedListener.OnLevelDataSourceChanged += TryInitLevelEditorAfterOneFrame;
        }

        public void Dispose()
        {
            LevelEditor?.Dispose();
            SceneView.duringSceneGui -= OnSceneGUI;
            LevelDataSourceChangedListener.OnLevelDataSourceChanged -= TryInitLevelEditorAfterOneFrame;
        }

        ~NonebEditor()
        {
            LevelEditor?.Dispose();
            SceneView.duringSceneGui -= OnSceneGUI;
            LevelDataSourceChangedListener.OnLevelDataSourceChanged -= TryInitLevelEditorAfterOneFrame;
        }

        private void OnSceneGUI(SceneView obj)
        {
            _toolbar.DrawSceneToolbar();
        }

        private static LevelDataSource? FindLevelDataSourceForActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            var allLevelDatas = AssetDatabase.FindAssets($"t:{nameof(LevelDataSource)}")
                                             .Select(AssetDatabase.GUIDToAssetPath)
                                             .Select(AssetDatabase.LoadAssetAtPath<LevelDataSource>);
            var matchingData = allLevelDatas.FirstOrDefault(s => s.SceneName == activeScene.name);

            return matchingData;
        }

        internal static void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            _instance?.TryInitLevelEditor();
        }

        internal static void Init()
        {
            CleanUpExistingInstance();

            _instance ??= new NonebEditor();
        }

        private static void CleanUpExistingInstance()
        {
            _instance?.Dispose();
            _instance = null;
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