using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editor.Maps.Grid;
using NonebNi.Editor.Maps.Toolbar;
using NonebNi.Editor.ServiceLocator;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps
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
        public static LevelEditor? Instance;
        private LevelEditorDataModel _dataModel = null!;
        private GridView _gridView = null!;
        private MapGenerationService _mapGenerationService = null!;
        private SceneToolbarView _toolbar = null!;

        public bool IsDrawGizmosOverlay = true;
        public bool IsDrawGridOverlay = true;

        public static bool IsInitialized => Instance != null;

        private void OnSceneGUI(SceneView view)
        {
            Handles.DrawLine(Vector3.back, Vector3.forward);

            _toolbar.DrawSceneToolbar();
            _gridView.DrawGrid();
        }

        #region INITIALIZATION / SERIALIZATION

        //methods are static so they can be initialized be the initializer.
        internal static void Init()
        {
            if (Instance == null)
                new LevelEditor().Initialize();
            else
                Instance.Initialize();
        }

        internal static void Destroy()
        {
            Instance?.UnregisterDelegates();

            Instance = null;
            SceneView.RepaintAll();
        }

        ~LevelEditor()
        {
            EditorApplication.delayCall += Destroy;
        }

        private void Initialize()
        {
            Instance = this;

            _mapGenerationService = NonebEditorServiceLocator.Instance.MapGenerationService;
            _dataModel = NonebEditorServiceLocator.Instance.LevelEditorDataModel;

            _gridView = new GridView();
            _toolbar = new SceneToolbarView();
            //todo:temporary implementation, need a window to generate level data per stage/level    
            var mapConfig = MapConfig.Create(10, 10);
            _dataModel.CurrentLevelData = new LevelData(
                mapConfig,
                WorldConfig.Create(Vector3.up, 1),
                _mapGenerationService.CreateEmptyMap(mapConfig)
            );

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