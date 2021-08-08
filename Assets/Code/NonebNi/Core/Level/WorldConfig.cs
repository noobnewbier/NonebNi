using System;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Core.Level
{
    [CreateAssetMenu(fileName = nameof(WorldConfig), menuName = MenuName.Data + nameof(WorldConfig))]
    public class WorldConfig : ScriptableObject
    {
        private static readonly Lazy<WorldConfig> LazyEmpty = new Lazy<WorldConfig>(() => Create(Vector3.zero, 0));

        [Range(0f, 10f)] [SerializeField] private float innerRadius;
        [SerializeField] private Vector3 upAxis = Vector3.up;
        [SerializeField] private Vector3 mapStartingPosition = Vector3.zero;
        public static WorldConfig Empty => LazyEmpty.Value;

        public static WorldConfig Create(Vector3 upAxis, float innerRadius)
        {
            var toReturn = CreateInstance<WorldConfig>();

            toReturn.upAxis = upAxis;
            toReturn.innerRadius = innerRadius;

            return toReturn;
        }

        public WorldConfigData ToData() => new WorldConfigData(innerRadius, mapStartingPosition, upAxis);
    }
}