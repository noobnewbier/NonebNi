using System;
using UnityEditor;

namespace NonebNi.LevelEditor.Common.Events
{
    /// <summary>
    /// Stripped from UnityEditor.ProGrids.PlayModeStateListener
    /// </summary>
    [InitializeOnLoad]
    public class PlayModeStateListener
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
        public static event Action? OnEnterPlayMode;
        public static event Action? OnExitPlayMode;
        public static event Action? OnEnterEditMode;
        public static event Action? OnExitEditMode;
        // ReSharper restore MemberCanBePrivate.Global
#pragma warning restore 649
    }
}