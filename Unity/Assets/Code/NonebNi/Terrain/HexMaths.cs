using UnityEngine;

namespace NonebNi.Terrain
{
    public static class HexMaths
    {
        public static Vector3[] FindCorners(float innerRadius)
        {
            var outerRadius = ToOuterRadius(innerRadius);

            return new[]
            {
                new Vector3(0f, 0f, outerRadius),
                new Vector3(innerRadius, 0f, 0.5f * outerRadius),
                new Vector3(innerRadius, 0f, -0.5f * outerRadius),
                new Vector3(0f, 0f, -outerRadius),
                new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
                new Vector3(-innerRadius, 0f, 0.5f * outerRadius)
            };
        }

        public static float ToOuterRadius(float innerRadius) =>
            // 0.866025 -> sqrt(3) / 2, read https://catlikecoding.com/unity/tutorials/hex-map/part-1/, session "about hexagons" for details
            innerRadius / 0.86602540378f;


        public static float SideDistanceOfHex(float innerRadius) => innerRadius * 2f;
        public static float UpDistanceOfHex(float innerRadius) => ToOuterRadius(innerRadius) * 1.5f;
    }
}