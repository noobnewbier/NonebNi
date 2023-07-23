using System;
using UnityEngine;

namespace NonebNi.Terrain
{
    [Serializable]
    public class TerrainConfigData
    {
        [SerializeField] private Vector3 mapStartingPosition;
        [SerializeField] private float innerRadius;

        public TerrainConfigData(float innerRadius, Vector3 mapStartingPosition)
        {
            this.innerRadius = innerRadius;
            this.mapStartingPosition = mapStartingPosition;
        }

        public Vector3 MapStartingPosition => mapStartingPosition;
        public float InnerRadius => innerRadius;

        // 0.866025 -> sqrt(3) / 2, read https://catlikecoding.com/unity/tutorials/hex-map/part-1/, session "about hexagons" for details
        public float OuterRadius => InnerRadius / 0.86602540378f;
    }
}