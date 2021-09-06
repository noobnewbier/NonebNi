using System;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Level
{
    [CreateAssetMenu(fileName = nameof(WorldConfigScriptable), menuName = MenuName.Data + nameof(WorldConfigScriptable))]
    public class WorldConfigScriptable : ScriptableObject
    {
        private static readonly Lazy<WorldConfigScriptable> LazyEmpty =
            new Lazy<WorldConfigScriptable>(() => Create(Vector3.zero, 0));

        [Range(0f, 10f)] [SerializeField] private float innerRadius;
        [SerializeField] private Vector3 upAxis = Vector3.up;
        [SerializeField] private Vector3 mapStartingPosition = Vector3.zero;
        public static WorldConfigScriptable Empty => LazyEmpty.Value;

        public static WorldConfigScriptable Create(Vector3 upAxis, float innerRadius)
        {
            var toReturn = CreateInstance<WorldConfigScriptable>();

            toReturn.upAxis = upAxis;
            toReturn.innerRadius = innerRadius;

            return toReturn;
        }

        public WorldConfigData CreateData() => new WorldConfigData(innerRadius, mapStartingPosition, upAxis);
    }
}