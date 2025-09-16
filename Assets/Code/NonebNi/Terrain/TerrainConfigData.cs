using System;
using UnityEngine;

namespace NonebNi.Terrain
{
    [Serializable]
    public record TerrainConfigData(float InnerRadius, Vector3 MapStartingPosition, float SolidFactor)
    {
        public float OuterRadius => HexMaths.ToOuterRadius(InnerRadius);

        /// <summary>
        ///     Factor of the blending region inside a hex cell.
        /// </summary>
        public float BlendFactor => 1 - SolidFactor;

        public Plane GridPlane => new(Vector3.up, MapStartingPosition);
    }
}