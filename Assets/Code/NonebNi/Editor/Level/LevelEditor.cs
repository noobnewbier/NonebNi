using System;
using NonebNi.Core.Level;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Inspector;
using NonebNi.Editor.Level.Maps;
using NonebNi.Editor.Level.Settings;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level
{
    public class LevelEditor : IDisposable
    {
        private readonly LevelEditorComponent _component;
        private readonly EntitiesPlacer _entitiesPlacer;

        private readonly MapView _mapView;
        private readonly TileInspectorView _tileInspectorView;

        public LevelEditor(Scene editedScene, LevelDataSource levelDataSource, INonebEditorComponent nonebEditorComponent)
        {
            _component = new LevelEditorComponent(new LevelEditorModule(levelDataSource, editedScene), nonebEditorComponent);

            _mapView = _component.MapView;
            _tileInspectorView = _component.TileInspectorView;

            _entitiesPlacer = new EntitiesPlacer(_component);

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            _mapView.OnSceneDraw();
            _tileInspectorView.OnSceneDraw();

            _entitiesPlacer.UpdateEntitiesPlacement();
        }

        public LevelEditorSettingsWindow GetSettingsWindow() => LevelEditorSettingsWindow.Init(_component);
    }
}