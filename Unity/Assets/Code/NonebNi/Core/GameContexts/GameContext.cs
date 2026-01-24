using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.Logs.Runtime;
using UnityEngine;

namespace NonebNi.Core.GameContexts
{
    /// <summary>
    /// KISS - atm it provides dependencies globally(basically Locator),
    /// but we might move on to a more sophisticated approach later where context can be gameobject/scene independent.
    /// </summary>
    public static class GameContext
    {
        private static Dictionary<Type, object> Datas = new ();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Datas = new ();
        }

        public static void Set<T>(T data) where T : class
        {
            Set(typeof(T), data);
        }

        public static void Set(Type type, object data)
        {
            if (Datas.ContainsKey(type)) Log.Warn("Context", "You are overwriting existing context, likely not what you want to do?");

            Datas[type] = data;
        }

        public static T? FindImmediate<T>()
        {
            var data = Datas.GetValueOrDefault(typeof(T));
            if (data is not T typedData)
            {
                Log.Error("Context", $"How the hell did you even end up here, we got a {data.GetType()} instead");
                return default;
            }

            return typedData;
        }

        public static T GetImmediate<T>()
        {
            var result = FindImmediate<T>();
            if (result == null) Log.Error("Context", $"Got null when looking for {typeof(T)}, this is why you get a NRE");

            // if a user get a null - he asked for it.
            return result!;
        }

        public static async UniTask<T> Get<T>() => await Get<T>(CancellationToken.None);

        public static async UniTask<T> Get<T>(CancellationToken ct)
        {
            while (true)
            {
                var (success, data) = Do();
                if (success) return data!;

                await UniTask.WaitForEndOfFrame(ct);
            }

            (bool success, T? data) Do()
            {
                if (!Datas.TryGetValue(typeof(T), out var data)) return default;

                if (data is not T typedData)
                {
                    Log.Error("Context", "How the hell did you even end up here");
                    return default;
                }

                return (true, typedData);
            }
        }
    }
}