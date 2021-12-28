using System;
using UnityEngine;

namespace NonebNi.Core.Level
{
    [Serializable]
    public class WorldConfigData
    {
        [SerializeField] private Vector3 mapStartingPosition;
        [SerializeField] private float innerRadius;

        public WorldConfigData(float innerRadius, Vector3 mapStartingPosition)
        {
            this.innerRadius = innerRadius;
            this.mapStartingPosition = mapStartingPosition;
        }

        public Vector3 MapStartingPosition => mapStartingPosition;
        public float InnerRadius => innerRadius;

        // 0.866025 -> sqrt(3) / 2, read https://catlikecoding.com/unity/tutorials/hex-map/part-1/, session "about hexagons" for details
        public float OuterRadius => InnerRadius / 0.86602540378f;

        //Origin from center, begin from top, rotate clockwise
        public Vector3[] TileCornersOffset => new[]
        {
            new Vector3(0f, 0f, OuterRadius),
            new Vector3(InnerRadius, 0f, 0.5f * OuterRadius),
            new Vector3(InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(0f, 0f, -OuterRadius),
            new Vector3(-InnerRadius, 0f, -0.5f * OuterRadius),
            new Vector3(-InnerRadius, 0f, 0.5f * OuterRadius)
        };
    }
}