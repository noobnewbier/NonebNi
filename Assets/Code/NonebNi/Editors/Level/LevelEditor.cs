using System;
using NonebNi.Editors.Di;
using NonebNi.Editors.Level.Data;
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
        private readonly LevelDataSyncer _levelDataSyncer;
        private readonly TileInspectorView _tileInspectorView;

        public LevelEditor(Scene editedScene, LevelDataSource levelDataSource, INonebEditorComponent nonebEditorComponent)
        {
            _component = new LevelEditorComponent(new LevelEditorModule(levelDataSource, editedScene), nonebEditorComponent);

            _gridView = _component.GridView;
            _tileInspectorView = _component.TileInspectorView;
            _errorOverviewView = _component.ErrorOverviewView;
            _entitiesPlacer = _component.EntitiesPlacer;
            _levelDataSyncer = _component.LevelDataSyncer;

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            _levelDataSyncer.Dispose();
        }

        private void OnSceneGUI(SceneView view)
        {
            _gridView.OnSceneDraw();

            const float paddingFromBottom = 20; //Magic value, no idea why the hack it's off by 20 constantly
            var sceneViewSize = SceneView.lastActiveSceneView.position.size;
            var position = new Vector2(0, sceneViewSize.y - TileInspectorView.WindowSize.y - paddingFromBottom);

            _tileInspectorView.OnSceneDraw(position);
            _errorOverviewView.OnSceneDraw(position + Vector2.right * TileInspectorView.WindowSize.x);

            _entitiesPlacer.UpdateEntitiesPlacement();
        }

        public LevelEditorSettingsWindow GetSettingsWindow() => LevelEditorSettingsWindow.Init(_component);
    }
}