using System;
using System.Linq;
using UnityEngine;

namespace NonebNi.Core.Diagnostics
{
    public static class Diagnostic
    {
        public static event Action<DRequest>? DRequestCreated;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            if (DRequestCreated == null) return;

            foreach (var @delegate in DRequestCreated.GetInvocationList().OfType<Action<DRequest>>()) DRequestCreated -= @delegate;
        }

        public static void Write(DRequest request)
        {
            DRequestCreated?.Invoke(request);
        }
    }
}