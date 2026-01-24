using UnityEngine;

namespace NonebNi.Core.Coordinates
{
    public static class StorageCoordinateExtensions
    {
        public static Coordinate ToAxial(this StorageCoordinate c) => ToAxial(c.X, c.Z);

        public static Coordinate ToAxial(int x, int z) => new(Mathf.CeilToInt(x - z / 2), z);
    }
}