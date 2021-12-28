using System;
using NonebNi.Core.Level;
using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level
{
    [CreateAssetMenu(fileName = nameof(WorldConfigSource), menuName = MenuName.Data + nameof(WorldConfigSource))]
    public class WorldConfigSource : ScriptableObject
    {
        private static readonly Lazy<WorldConfigSource> LazyEmpty =
            new Lazy<WorldConfigSource>(() => Create(0, Vector3.zero));

        [Range(0f, 10f)] [SerializeField] private float innerRadius;
        [SerializeField] private Vector3 mapStartingPosition = Vector3.zero;
        public static WorldConfigSource Empty => LazyEmpty.Value;

        public static WorldConfigSource Create(float innerRadius, Vector3 mapStartingPosition)
        {
            var toReturn = CreateInstance<WorldConfigSource>();

            toReturn.innerRadius = innerRadius;
            toReturn.mapStartingPosition = mapStartingPosition;

            return toReturn;
        }

        public static WorldConfigSource Create(WorldConfigData data) => Create(data.InnerRadius, data.MapStartingPosition);

        public WorldConfigData CreateData() => new WorldConfigData(innerRadius, mapStartingPosition);

        public void CopyFromData(WorldConfigData worldConfigData)
        {
            innerRadius = worldConfigData.InnerRadius;
            mapStartingPosition = worldConfigData.MapStartingPosition;

            EditorUtility.SetDirty(this);
        }
    }
}