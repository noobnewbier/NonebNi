using System;
using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Error;
using NonebNi.Editor.Level.Inspector;
using NonebNi.Editor.Level.Maps;
using NonebNi.Editor.Level.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level
{
    public class LevelEditor : IDisposable
    {
        private readonly LevelEditorComponent _component;
        private readonly EntitiesPlacer _entitiesPlacer;
        private readonly ErrorOverviewView _errorOverviewView;

        private readonly GridView _gridView;
        private readonly TileInspectorView _tileInspectorView;

        public LevelEditor(Scene editedScene, LevelDataSource levelDataSource, INonebEditorComponent nonebEditorComponent)
        {
            _component = new LevelEditorComponent(new LevelEditorModule(levelDataSource, editedScene), nonebEditorComponent);

            _gridView = _component.GridView;
            _tileInspectorView = _component.TileInspectorView;
            _errorOverviewView = _component.ErrorOverviewView;
            _entitiesPlacer = _component.EntitiesPlacer;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            _gridView.OnSceneDraw();

            var sceneViewSize = SceneView.lastActiveSceneView.position.size;
            var position = new Vector2(0, sceneViewSize.y - TileInspectorView.WindowSize.y);
            _tileInspectorView.OnSceneDraw(position);
            _errorOverviewView.OnSceneDraw(position + Vector2.down * ErrorOverviewView.WindowSize.y);

            _entitiesPlacer.UpdateEntitiesPlacement();
        }

        public LevelEditorSettingsWindow GetSettingsWindow() => LevelEditorSettingsWindow.Init(_component);
    }
}