using NonebNi.Editor.Maps.Toolbar;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps
{
    [InitializeOnLoad]
    internal static class MapEditorInitializer
    {
        /// <summary>
        /// Remembers whether or not ProGrids was open when Unity was shut down last. This happens when Unity opens.
        /// </summary>
        static MapEditorInitializer()
        {
            MapEditor.Init();
            PlayModeStateListener.OnEnterEditMode += MapEditor.Init;
            PlayModeStateListener.OnEnterPlayMode += MapEditor.Destroy;
        }
    }

    public class MapEditor
    {
        public static MapEditor? Instance;
        public bool IsDrawGizmosOverlay = true;
        public bool IsDrawGridOverlay = true;

        private void OnSceneGUI(SceneView view)
        {
            Handles.DrawLine(Vector3.back, Vector3.forward);

            var toolbar = new MapEditorSceneToolbarView();
            toolbar.DrawSceneToolbar();
        }

        #region INITIALIZATION / SERIALIZATION

        //methods are static so they can be initialized be the initializer.
        internal static void Init()
        {
            if (Instance == null)
                new MapEditor().Initialize();
            else
                Instance.Initialize();
        }

        internal static void Destroy()
        {
            Instance?.UnregisterDelegates();

            Instance = null;
            SceneView.RepaintAll();
        }

        ~MapEditor()
        {
            EditorApplication.delayCall += Destroy;
        }

        private void Initialize()
        {
            Instance = this;
            RegisterDelegates();
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