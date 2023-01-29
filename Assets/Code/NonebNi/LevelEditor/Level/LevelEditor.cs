using System;
using NonebNi.LevelEditor.Di;
using NonebNi.LevelEditor.Level.Data;
using NonebNi.LevelEditor.Level.Entities;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Inspector;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Settings;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Level
{
    public class LevelEditor : IDisposable
    {
        private readonly LevelEditorComponent _component;

        private readonly EntitiesPlacer _entitiesPlacer;
        private readonly ErrorOverviewView _errorOverviewView;
        private readonly GridView _gridView;
        private readonly LevelDataSyncer _levelDataSyncer;
        private readonly TileInspectorView _tileInspectorView;

        public LevelEditor(Scene editedScene,
                           EditorLevelDataSource editorLevelDataSource,
                           INonebEditorComponent nonebEditorComponent)
        {
            _component = new LevelEditorComponent(
                new LevelEditorModule(editorLevelDataSource, editedScene),
                nonebEditorComponent
            );

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
            _errorOverviewView.OnSceneDraw(position + Vector2.right * TileInspectorView.WindowSize.x, view);

            _entitiesPlacer.UpdateEntitiesPlacement();
        }

        public LevelEditorSettingsWindow GetSettingsWindow() => LevelEditorSettingsWindow.Init(_component);
    }
}