using System;
using System.Collections.Generic;
using NonebNi.Core.Maps;

namespace NonebNi.Core.Coordinates
{
    public interface ICoordinateService
    {
        Coordinate GetAxialCoordinateFromNestedArrayIndex(int x, int z);
        Coordinate GetCoordinateFromFlattenArrayIndex(int index, MapConfig mapConfig);
        int GetFlattenArrayIndexFromAxialCoordinate(int x, int z, MapConfig config);

        IReadOnlyList<Coordinate> GetFlattenCoordinates(MapConfig mapConfig);
    }

    public class CoordinateService : ICoordinateService
    {
        public Coordinate GetAxialCoordinateFromNestedArrayIndex(int x, int z)
        {
            var axialX = x + z % 2 + z / 2;
            var axialZ = z;
            return new Coordinate(axialX, axialZ);
        }

        public Coordinate GetCoordinateFromFlattenArrayIndex(int index, MapConfig config)
        {
            if (index > config.GetTotalMapSize() || index < 0)
                throw new ArgumentOutOfRangeException($"{index} is out of range of the given config: ${config}");

            var nestedArrayZ = index / config.GetMap2DActualHeight();
            var nestedArrayX = index - nestedArrayZ * config.GetMap2DActualWidth();

            return GetAxialCoordinateFromNestedArrayIndex(nestedArrayX, nestedArrayZ);
        }

        public int GetFlattenArrayIndexFromAxialCoordinate(int x, int z, MapConfig config)
        {
            if (x > config.GetMap2DArrayWidth() || z > config.GetMap2DArrayHeight())
                throw new ArgumentOutOfRangeException($"{x} or {z} is out of range of the given config: ${config}");

            return z * config.GetMap2DActualWidth() + x - z % 2 - z / 2;
        }

        public IReadOnlyList<Coordinate> GetFlattenCoordinates(MapConfig mapConfig)
        {
            var toReturn = new List<Coordinate>();

            for (var i = 0; i < mapConfig.GetTotalMapSize(); i++) toReturn.Add(GetCoordinateFromFlattenArrayIndex(i, mapConfig));

            return toReturn;
        }
    }
}