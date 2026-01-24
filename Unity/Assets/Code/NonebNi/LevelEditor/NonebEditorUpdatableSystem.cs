using System;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityUtils;

namespace NonebNi.LevelEditor
{
    public class NonebEditorUpdatableSystem
    {
        public static event Action? Update;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            PlayerLoopHelpers.AddRunnerToPlayerLoop(typeof(Update), typeof(NonebEditorUpdatableSystem), UpdateFunc);
        }

        private static void UpdateFunc()
        {
            Update?.Invoke();
        }
    }
}