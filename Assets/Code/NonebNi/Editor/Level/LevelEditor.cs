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
            var component = new LevelEditorComponent(new LevelEditorModule(levelData), nonebEditorComponent);

            _mapView = new MapView(component);
            _entitiesPlacer = new EntitiesPlacer(component);

            LevelEditorModel model = component.LevelEditorModel;
            model.Map = new Map(levelData.MapConfig);

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