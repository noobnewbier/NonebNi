using UnityEngine;

namespace NonebNi.Terrain
{
    /// <summary>
    ///     Set of five vertex positions describing a cell edge.
    /// </summary>
    public struct EdgeVertices
    {
        public Vector3 V1, V2, V3, V4, V5;

        /// <summary>
        ///     Create a straight edge with equidistant vertices between two corner positions.
        /// </summary>
        /// <param name="corner1">Frist corner.</param>
        /// <param name="corner2">Second corner.</param>
        public EdgeVertices(Vector3 corner1, Vector3 corner2)
        {
            V1 = corner1;
            V2 = Vector3.Lerp(corner1, corner2, 0.25f);
            V3 = Vector3.Lerp(corner1, corner2, 0.5f);
            V4 = Vector3.Lerp(corner1, corner2, 0.75f);
            V5 = corner2;
        }
    }
}