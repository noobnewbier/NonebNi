using System;
using NonebNi.Editors.Di;
using NonebNi.Editors.Level.Entities;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Inspector;
using NonebNi.Editors.Level.Maps;
using NonebNi.Editors.Level.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Level
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