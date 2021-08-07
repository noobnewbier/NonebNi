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

        public bool IsDrawMapOverlay = true;

        private void OnSceneGUI(SceneView view)
        {
            if (!IsDrawMapOverlay) return;

            Handles.DrawLine(Vector3.back, Vector3.forward);

            var toolbar = new MapEditorSceneToolbarView();
            toolbar.DrawSceneToolbar();
        }

        #region INITIALIZATION / SERIALIZATION

        internal static void Init()
        {
            if (Instance == null)
                new MapEditor().Initialize();
            else
                Instance.Initialize();
        }

        ~MapEditor()
        {
            EditorApplication.delayCall += Destroy;
        }

        internal static void Destroy()
        {
            Instance?.UnregisterDelegates();

            Instance = null;
            SceneView.RepaintAll();
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