using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps
{
    [InitializeOnLoad]
    static class MapEditorInitializer
    {
        /// <summary>
        /// Remembers whether or not ProGrids was open when Unity was shut down last. This happens when Unity opens.
        /// </summary>
        static MapEditorInitializer()
        {
            MapEditor.InitIfEnabled();
            PlayModeStateListener.OnEnterEditMode += MapEditor.InitIfEnabled;
            PlayModeStateListener.OnEnterPlayMode += MapEditor.DestroyIfEnabled;
        }
    }

    public class MapEditor
    {
        private static MapEditor? _instance;


        #region INITIALIZATION / SERIALIZATION

        internal static void InitIfEnabled()
        {
            // if (!EditorApplication.isPlayingOrWillChangePlaymode && EditorPrefs.GetBool(PreferenceKeys.ProGridsIsEnabled))
                Init();
        }

        internal static void DestroyIfEnabled()
        {
            _instance?.Destroy();
        }

        internal static void Init()
        {
            //Todo: adding preference keys
            // EditorPrefs.SetBool(PreferenceKeys.ProGridsIsEnabled, true);

            if (_instance == null)
                new MapEditor().Initialize();
            else
                _instance.Initialize();
        }

        ~MapEditor()
        {
            EditorApplication.delayCall += Destroy;
        }

        void Initialize()
        {
            _instance = this;
            RegisterDelegates();
        }

        internal static void Close()
        {
            // EditorPrefs.SetBool(PreferenceKeys.ProGridsIsEnabled, false);

            _instance?.Destroy();
        }

        void Destroy()
        {
            UnregisterDelegates();

            _instance = null;
            SceneView.RepaintAll();
        }

        void RegisterDelegates()
        {
            UnregisterDelegates();

            SceneView.duringSceneGui += OnSceneGUI;
            // SceneView.duringSceneGui += DrawSceneGrid;

            // Undo.undoRedoPerformed += ResetActiveTransformValues;
        }

        void UnregisterDelegates()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            // SceneView.duringSceneGui -= DrawSceneGrid;

            // Undo.undoRedoPerformed -= ResetActiveTransformValues;
        }

        #endregion

        void OnSceneGUI(SceneView view)
        {
            Handles.DrawLine(Vector3.back, Vector3.forward);
        }
    }
}