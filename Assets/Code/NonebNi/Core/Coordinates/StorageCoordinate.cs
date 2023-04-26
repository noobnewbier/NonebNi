namespace NonebNi.Core.Coordinates
{
    /// <summary>
    /// Representing an Axial coordinate in a form that can be easily accessed in a jagged array.
    ///
    /// Ref: https://www.redblobgames.com/grids/hexagons/#map-storage
    /// </summary>
    public readonly struct StorageCoordinate
    {
        public readonly int X;
        public readonly int Z;

        public StorageCoordinate(int x, int z)
        {
            X = x;
            Z = z;
        }

        public static StorageCoordinate StorageCoordinateFromIndex(int i, int width)
        {
            var z = i / width;
            var x = i - z;

            return new StorageCoordinate(x, z);
        }

        public static StorageCoordinate FromAxial(Coordinate coordinate)
        {
            var z = coordinate.Z;
            var x = coordinate.X + z / 2;
            return new StorageCoordinate(x, z);
        }

        public static int Get1DArrayIndexFromStorageCoordinate(StorageCoordinate storageCoordinate, int width) =>
            Get1DArrayIndexFromStorageCoordinate(storageCoordinate.X, storageCoordinate.Z, width);

        public static int Get1DArrayIndexFromStorageCoordinate(int x, int z, int width) => x + z * width;
    }
}