using NonebNi.Editors.Level;
using UnityEditor;

namespace NonebNi.Editors
{
    /// <summary>
    /// Managing "editor session", can be seen as the entry point of every single editor session.
    /// An "editor session" is the time between the time of entering edit mode and exiting edit mode.
    /// </summary>
    public class SessionManager
    {
        private NonebEditor? _nonebEditor;

        private void StartSession()
        {
            _nonebEditor = new NonebEditor();
        }

        private void EndSession()
        {
            _nonebEditor?.Dispose();
            _nonebEditor = null;
        }

        [InitializeOnLoad]
        internal static class Initializer
        {
            private static readonly SessionManager? Instance;

            static Initializer()
            {
                if (Instance == null)
                {
                    Instance = new SessionManager();

                    PlayModeStateListener.OnEnterEditMode += Instance.StartSession;
                    PlayModeStateListener.OnExitEditMode += Instance.EndSession;

                    Instance.StartSession();
                }
            }
        }
    }
}