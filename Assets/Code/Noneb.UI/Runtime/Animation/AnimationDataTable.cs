using System;
using UnityEngine;
using UnityUtils.Serialization;

namespace Noneb.UI.Animation
{
    /// <summary>
    /// We might want custom drawer later, but for now this suffices.
    /// </summary>
    [Serializable]
    public record AnimationDataTable
    {
        [SerializeField] private SerializableDictionary<string, AnimationData> animIdAndData = new();

        public AnimationData? FindAnim(string id)
        {
            if (!animIdAndData.TryGetValue(id, out var result)) return null;

            return result;
        }
    }
}