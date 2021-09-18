using System;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Maps;
using UnityEditor;

namespace NonebNi.Editor.Level
{
    public class LevelEditor : IDisposable
    {
        private readonly EntitiesPlacer _entitiesPlacer;
        private readonly MapView _mapView;

        public LevelEditor(LevelData levelData, INonebEditorComponent nonebEditorComponent)
        {
            var map = new Map(levelData.MapConfig);
            var component = new LevelEditorComponent(new LevelEditorModule(levelData, map), nonebEditorComponent);

            _mapView = new MapView(component);
            _entitiesPlacer = new EntitiesPlacer(component);

            SceneView.duringSceneGui += OnSceneGUI;
        }

        public void Dispose()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            _mapView.OnSceneDraw();
            _entitiesPlacer.UpdateEntitiesPlacement();
        }
    }
}