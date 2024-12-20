using NonebNi.LevelEditor.Common.Events;
using NonebNi.LevelEditor.Di;
using NonebNi.LevelEditor.Inspectors;
using StrongInject;
using UnityEditor;

namespace NonebNi.LevelEditor
{
    /// <summary>
    ///     Managing "session", can be seen as the entry point of every single inspecting/editing session.
    ///     A "session" is the time between the time of:
    ///         1. entering edit mode and exiting edit mode.
    ///         2. entering play mode and exiting play mode.
    /// </summary>
    public class SessionManager
    {
        private Owned<NonebEditor>? _nonebEditor;
        private Owned<NonebInspector>? _nonebInspector;

        private void StartEditing()
        {
            _nonebEditor = new NonebEditorContainer().Resolve();
        }

        private void StopEditing()
        {
            _nonebEditor?.Dispose();
            _nonebEditor = null;
        }

        private void StopInspecting()
        {
            _nonebInspector?.Dispose();
        }

        private void StartInspecting()
        {
            _nonebInspector = new NonebInspectorContainer().Resolve();
        }

        [InitializeOnLoad]
        internal static class Initializer
        {
            private static readonly SessionManager? Instance;

            static Initializer()
            {
                if (Instance != null) return;

                Instance = new SessionManager();

                PlayModeStateListener.OnEnterEditMode += Instance.StartEditing;
                PlayModeStateListener.OnExitEditMode += Instance.StopEditing;

                PlayModeStateListener.OnEnterPlayMode += Instance.StartInspecting;
                PlayModeStateListener.OnExitPlayMode += Instance.StopInspecting;

                if (EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    Instance.StartInspecting();
                }
                else
                {
                    Instance.StartEditing();
                }
            }
        }
    }
}