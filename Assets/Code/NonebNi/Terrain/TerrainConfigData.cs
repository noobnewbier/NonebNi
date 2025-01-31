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
        public float OuterRadius => HexMaths.ToOuterRadius(InnerRadius);

        public Plane GridPlane => new(Vector3.up, mapStartingPosition);
    }
}