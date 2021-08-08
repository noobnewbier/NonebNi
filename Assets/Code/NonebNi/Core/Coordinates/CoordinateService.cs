using System;
using System.Collections.Generic;
using NonebNi.Core.Maps;

namespace NonebNi.Core.Coordinates
{
    public interface ICoordinateService
    {
        Coordinate GetAxialCoordinateFromNestedArrayIndex(int x, int z);
        Coordinate GetCoordinateFromFlattenArrayIndex(int index, MapConfigScriptable mapConfigScriptable);
        int GetFlattenArrayIndexFromAxialCoordinate(int x, int z, MapConfigScriptable configScriptable);

        IReadOnlyList<Coordinate> GetFlattenCoordinates(MapConfigScriptable mapConfigScriptable);
    }

    public class CoordinateService : ICoordinateService
    {
        public Coordinate GetAxialCoordinateFromNestedArrayIndex(int x, int z)
        {
            var axialX = x + z % 2 + z / 2;
            var axialZ = z;
            return new Coordinate(axialX, axialZ);
        }

        public Coordinate GetCoordinateFromFlattenArrayIndex(int index, MapConfigScriptable configScriptable)
        {
            if (index > configScriptable.GetTotalMapSize() || index < 0)
                throw new ArgumentOutOfRangeException($"{index} is out of range of the given config: ${configScriptable}");

            var nestedArrayZ = index / configScriptable.GetMap2DActualHeight();
            var nestedArrayX = index - nestedArrayZ * configScriptable.GetMap2DActualWidth();

            return GetAxialCoordinateFromNestedArrayIndex(nestedArrayX, nestedArrayZ);
        }

        public int GetFlattenArrayIndexFromAxialCoordinate(int x, int z, MapConfigScriptable configScriptable)
        {
            if (x > configScriptable.GetMap2DArrayWidth() || z > configScriptable.GetMap2DArrayHeight())
                throw new ArgumentOutOfRangeException($"{x} or {z} is out of range of the given config: ${configScriptable}");

            return z * configScriptable.GetMap2DActualWidth() + x - z % 2 - z / 2;
        }

        public IReadOnlyList<Coordinate> GetFlattenCoordinates(MapConfigScriptable mapConfigScriptable)
        {
            var toReturn = new List<Coordinate>();

            for (var i = 0; i < mapConfigScriptable.GetTotalMapSize(); i++) toReturn.Add(GetCoordinateFromFlattenArrayIndex(i, mapConfigScriptable));

            return toReturn;
        }
    }
}