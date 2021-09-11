﻿using System;
using System.Linq;
using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level;
using NonebNi.Editor.Toolbar;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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
            EditorSceneManager.activeSceneChangedInEditMode += NonebEditor.ReloadLevelData;
        }
    }

    public class NonebEditor : IDisposable
    {
        private static NonebEditor? _instance;
        private readonly NonebEditorComponent _component;
        private readonly NonebEditorModel _model;

        private readonly SceneToolbarView _toolbar;

        private LevelEditor? _levelEditor;

        private NonebEditor()
        {
            _instance = this;

            _component = new NonebEditorComponent(new NonebEditorModule());
            _model = _component.NonebEditorModel;
            _model.LevelDataSource = FindLevelDataSourceForActiveScene();

            _toolbar = new SceneToolbarView(_component);

            SceneView.duringSceneGui += OnSceneGUI;
            _model.OnLevelDataSourceChanged += OnLevelDataSourceChanged;

            InitLevelEditor();
        }

        public void Dispose()
        {
            _levelEditor?.Dispose();
            SceneView.duringSceneGui -= OnSceneGUI;
            _model.OnLevelDataSourceChanged -= OnLevelDataSourceChanged;
        }

        private void OnSceneGUI(SceneView obj)
        {
            Debug.Log(GetHashCode());

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

        internal static void ReloadLevelData(Scene arg0, Scene arg1)
        {
            _instance?.UpdateLevelDataSourceToMatchActiveScene();
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

        ~NonebEditor()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            _model.OnLevelDataSourceChanged -= OnLevelDataSourceChanged;
        }

        private void OnLevelDataSourceChanged(LevelDataSource? dataSource)
        {
            InitLevelEditor();
        }

        private void InitLevelEditor()
        {
            _levelEditor?.Dispose();
            _levelEditor = null;

            if (_model.LevelDataSource != null && _model.LevelDataSource.IsValid)
                _levelEditor = new LevelEditor(_model.LevelDataSource.CreateData()!, _component);
        }

        private void UpdateLevelDataSourceToMatchActiveScene()
        {
            _model.LevelDataSource = FindLevelDataSourceForActiveScene();
        }
    }
}