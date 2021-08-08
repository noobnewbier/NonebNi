using UnityEngine;

namespace NonebNi.Core.Level
{
    public class WorldConfigData
    {
        public Vector3 MapStartingPosition { get; }
        public Vector3 UpAxis { get; }
        public float InnerRadius { get; }

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

        public WorldConfigData(float innerRadius, Vector3 mapStartingPosition, Vector3 upAxis)
        {
            InnerRadius = innerRadius;
            MapStartingPosition = mapStartingPosition;
            UpAxis = upAxis;
        }
    }
}