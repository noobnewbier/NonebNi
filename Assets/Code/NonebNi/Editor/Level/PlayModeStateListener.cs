using System;
using UnityEditor;

namespace NonebNi.Editor.Level
{
    /// <summary>
    /// Stripped from UnityEditor.ProGrids.PlayModeStateListener
    /// </summary>
    [InitializeOnLoad]
    internal class PlayModeStateListener
    {
        static PlayModeStateListener()
        {
            EditorApplication.playModeStateChanged += x =>
            {
                switch (x)
                {
                    case PlayModeStateChange.EnteredEditMode:
                        OnEnterEditMode?.Invoke();
                        break;
                    case PlayModeStateChange.ExitingEditMode:
                        OnExitEditMode?.Invoke();
                        break;
                    case PlayModeStateChange.EnteredPlayMode:
                        OnEnterPlayMode?.Invoke();
                        break;
                    case PlayModeStateChange.ExitingPlayMode:
                        OnExitPlayMode?.Invoke();
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }
            };
        }
#pragma warning disable 649
        // ReSharper disable MemberCanBePrivate.Global
        internal static event Action? OnEnterPlayMode;
        internal static event Action? OnExitPlayMode;
        internal static event Action? OnEnterEditMode;
        internal static event Action? OnExitEditMode;
        // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649
    }
}