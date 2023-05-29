using System;
using NonebNi.LevelEditor.Level.Entities;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.LevelEditor.Level.Settings;
using NonebNi.LevelEditor.Level.Tiles;
using UnityEditor;
using UnityEngine;

namespace NonebNi.LevelEditor.Level
{
    public class LevelEditor : IDisposable
    {
        private readonly EditorEntitySyncService _editorEntitySyncService;
        private readonly LevelEditorModel _editorModel;
        private readonly EntitiesPlacer _entitiesPlacer;
        private readonly ErrorOverviewView _errorOverviewView;
        private readonly GridView _gridView;
        private readonly LevelDataSyncer _levelDataSyncer;
        private readonly LevelSavingService _levelSavingService;
        private readonly TileInspectorView _tileInspectorView;

        public LevelEditor(GridView gridView,
            TileInspectorView tileInspectorView,
            ErrorOverviewView errorOverviewView,
            EntitiesPlacer entitiesPlacer,
            LevelDataSyncer levelDataSyncer,
            LevelEditorModel editorModel,
            EditorEntitySyncService editorEntitySyncService,
            LevelSavingService levelSavingService)
        {
            _gridView = gridView;
            _tileInspectorView = tileInspectorView;
            _errorOverviewView = errorOverviewView;
            _entitiesPlacer = entitiesPlacer;
            _levelDataSyncer = levelDataSyncer;
            _editorModel = editorModel;
            _editorEntitySyncService = editorEntitySyncService;
            _levelSavingService = levelSavingService;

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

        public LevelEditorSettingsWindow CreateSettingsWindow() =>
            LevelEditorSettingsWindow.Init(
                _editorModel,
                _editorEntitySyncService,
                _levelSavingService
            );
    }
}