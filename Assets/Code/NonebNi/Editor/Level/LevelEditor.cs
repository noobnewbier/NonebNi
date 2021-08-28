using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Di;
using NonebNi.Editor.Level.Entities;
using NonebNi.Editor.Level.Maps;
using NonebNi.Editor.Level.Toolbar;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level
{
    [InitializeOnLoad]
    internal static class LevelEditorInitializer
    {
        static LevelEditorInitializer()
        {
            LevelEditor.Init();
            PlayModeStateListener.OnEnterEditMode += LevelEditor.Init;
            PlayModeStateListener.OnEnterPlayMode += LevelEditor.Destroy;
        }
    }

    public class LevelEditor
    {
        private static LevelEditor? _instance;
        private LevelEditorDataModel _dataModel = null!;
        private EntitiesPlacer _entitiesPlacer = null!;
        private MapView _mapView = null!;
        private SceneToolbarView _toolbar = null!;

        private void OnSceneGUI(SceneView view)
        {
            _toolbar.DrawSceneToolbar();
            _mapView.OnSceneDraw();
            _entitiesPlacer.UpdateEntitiesPlacement();
        }

        #region INITIALIZATION / SERIALIZATION

        //methods are static so they can be initialized be the initializer.
        internal static void Init()
        {
            if (_instance == null)
                new LevelEditor().Initialize();
            else
                _instance.Initialize();
        }

        internal static void Destroy()
        {
            _instance?.UnregisterDelegates();

            _instance = null;
            SceneView.RepaintAll();
        }

        ~LevelEditor()
        {
            UnregisterDelegates();
        }

        private void Initialize()
        {
            _instance = this;


            //todo:temporary implementation, need a window to generate level data per stage/level    
            var mapConfig = new MapConfigData(10, 10);
            var worldConfig = new WorldConfigData(1, Vector3.zero, Vector3.up);
            var levelData = new LevelData(mapConfig, worldConfig, new Map(mapConfig));

            var module = new LevelEditorModule(levelData);
            var component = new LevelEditorComponent(module);

            _mapView = new MapView(component);
            _toolbar = new SceneToolbarView(component);
            _entitiesPlacer = new EntitiesPlacer(component);
            _dataModel = component.LevelEditorDataModel;

            RegisterDelegates();

            _dataModel.IsGizmosVisible = true;
            _dataModel.IsGridVisible = true;
        }

        private void RegisterDelegates()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            // Undo.undoRedoPerformed += ResetActiveTransformValues;
        }

        private void UnregisterDelegates()
        {
            SceneView.duringSceneGui -= OnSceneGUI;

            // Undo.undoRedoPerformed -= ResetActiveTransformValues;
        }

        #endregion
    }
}