using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.PlayerLoop;

namespace NonebNi.LevelEditor
{
    public class NonebUpdatableSystem
    {
        public static event Action? Update;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void Initialize()
        {
            var newPlayerLoopRoot = PlayerLoop.GetCurrentPlayerLoop();

            InsertLoop(newPlayerLoopRoot, typeof(Update), typeof(NonebUpdatableSystem), UpdateFunc);
            PlayerLoop.SetPlayerLoop(newPlayerLoopRoot);
        }

        private static void UpdateFunc()
        {
            Update?.Invoke();
        }

        private static int FindLoopSystemIndex(IList<PlayerLoopSystem> playerLoopList, Type systemType)
        {
            for (var i = 0; i < playerLoopList.Count; i++)
                if (playerLoopList[i].type == systemType)
                    return i;

            throw new Exception("Target PlayerLoopSystem does not found. Type:" + systemType.FullName);
        }

        private static PlayerLoopSystem[] InsertRunner(
            PlayerLoopSystem loopSystem,
            Type loopRunnerType,
            PlayerLoopSystem.UpdateFunction loopRunnerDelegate)
        {
            var source = loopSystem.subSystemList;

            var dest = new PlayerLoopSystem[source.Length + 1];
            Array.Copy(source, 0, dest, 0, source.Length);

            dest[^1].type = loopRunnerType;
            dest[^1].updateDelegate = loopRunnerDelegate;

            return dest;
        }

        private static void InsertLoop(
            PlayerLoopSystem toLoopSystem,
            Type intoLoopType,
            Type newRunnerType,
            PlayerLoopSystem.UpdateFunction newRunnerDelegate)
        {
            var subSystemList = toLoopSystem.subSystemList;
            var i = FindLoopSystemIndex(subSystemList, intoLoopType);

            subSystemList[i].subSystemList = InsertRunner(
                subSystemList[i],
                newRunnerType,
                newRunnerDelegate
            );
        }
    }
}