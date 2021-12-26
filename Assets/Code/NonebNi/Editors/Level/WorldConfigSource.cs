using System;
using NonebNi.Game.Level;
using UnityEditor;
using UnityEngine;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level
{
    [CreateAssetMenu(fileName = nameof(WorldConfigSource), menuName = MenuName.Data + nameof(WorldConfigSource))]
    public class WorldConfigSource : ScriptableObject
    {
        private static readonly Lazy<WorldConfigSource> LazyEmpty =
            new Lazy<WorldConfigSource>(() => Create(Vector3.up, 0, Vector3.zero));

        [Range(0f, 10f)] [SerializeField] private float innerRadius;
        [SerializeField] private Vector3 upAxis = Vector3.up;
        [SerializeField] private Vector3 mapStartingPosition = Vector3.zero;
        public static WorldConfigSource Empty => LazyEmpty.Value;

        public static WorldConfigSource Create(Vector3 upAxis, float innerRadius, Vector3 mapStartingPosition)
        {
            var toReturn = CreateInstance<WorldConfigSource>();

            toReturn.upAxis = upAxis;
            toReturn.innerRadius = innerRadius;
            toReturn.mapStartingPosition = mapStartingPosition;

            return toReturn;
        }

        public static WorldConfigSource Create(WorldConfigData data) => Create(
            data.UpAxis,
            data.InnerRadius,
            data.MapStartingPosition
        );

        public WorldConfigData CreateData() => new WorldConfigData(innerRadius, mapStartingPosition, upAxis);

        public void CopyFromData(WorldConfigData worldConfigData)
        {
            innerRadius = worldConfigData.InnerRadius;
            upAxis = worldConfigData.UpAxis;
            mapStartingPosition = worldConfigData.MapStartingPosition;

            EditorUtility.SetDirty(this);
        }
    }
}