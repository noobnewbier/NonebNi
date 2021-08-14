using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Level.Map;
using NonebNi.Editor.Level.Toolbar;
using NonebNi.Editor.ServiceLocator;
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
        private MapGenerationService _mapGenerationService = null!;
        private MapView _mapView = null!;
        private SceneToolbarView _toolbar = null!;

        private void OnSceneGUI(SceneView view)
        {
            _toolbar.DrawSceneToolbar();
            _mapView.OnSceneDraw();
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
            EditorApplication.delayCall += Destroy;
        }

        private void Initialize()
        {
            _instance = this;

            _mapGenerationService = NonebEditorServiceLocator.Instance.MapGenerationService;
            _dataModel = NonebEditorServiceLocator.Instance.LevelEditorDataModel;

            _mapView = new MapView();
            _toolbar = new SceneToolbarView();

            //todo:temporary implementation, need a window to generate level data per stage/level    
            var mapConfig = new MapConfigData(10, 10);
            var worldConfig = new WorldConfigData(1, Vector3.zero, Vector3.up);
            _dataModel.CurrentLevelData = new LevelData(mapConfig, worldConfig, _mapGenerationService.CreateEmptyMap(mapConfig));

            RegisterDelegates();

            _dataModel.IsGizmosVisible = true;
            _dataModel.IsGridVisible = true;
        }

        private void RegisterDelegates()
        {
            UnregisterDelegates();

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