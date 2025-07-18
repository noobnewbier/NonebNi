using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Logging;

namespace NonebNi.Ui.UIContexts
{
    /// <summary>
    /// KISS - atm it provides dependencies globally(basically Locator),
    /// but we might move on to a more sophisticated approach later where context can be gameobject/scene independent.
    /// </summary>
    public static class UIContext
    {
        private static readonly Dictionary<Type, object> Datas = new();

        public static void Set<T>(T data) where T : class
        {
            Set(typeof(T), data);
        }

        public static void Set(Type type, object data)
        {
            if (Datas.ContainsKey(type)) Log.Warning("You are overwriting existing context, likely not what you want to do?");

            Datas[type] = data;
        }

        public static async UniTask<T> Get<T>(CancellationToken ct = default)
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
                    Log.Error("How the hell did you even end up here");
                    return default;
                }

                return (true, typedData);
            }
        }
    }
}