using System;
using System.Collections.Generic;
using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;

namespace NonebNi.Core.Maps
{
    /*
     * the storage is in axial coordinate with grid(jagged array) for easy access.
     */
    public class Map
    {
        private readonly Tile?[,] _tileGrid;
        private readonly Unit?[,] _unitGrid;

        public Map(IEnumerable<Tile> tiles,
                   IEnumerable<Unit> units,
                   MapConfig mapConfig)
        {
            var map2DArrayWidth = mapConfig.GetMap2DArrayWidth();
            var map2dArrayHeight = mapConfig.GetMap2DArrayHeight();
            _tileGrid = CreateGrid(tiles, map2DArrayWidth, map2dArrayHeight);
            _unitGrid = CreateGrid(units, map2DArrayWidth, map2dArrayHeight);
        }

        public Map(IEnumerable<Tile> tiles, MapConfig mapConfig)
        {
            var map2DArrayWidth = mapConfig.GetMap2DArrayWidth();
            var map2dArrayHeight = mapConfig.GetMap2DArrayHeight();
            _tileGrid = CreateGrid(tiles, map2DArrayWidth, map2dArrayHeight);
            _unitGrid = CreateGrid(new List<Unit>(), map2DArrayWidth, map2dArrayHeight);
        }


        private static T[,] CreateGrid<T>(IEnumerable<T> boardItems, int map2DArrayWidth, int map2dArrayHeight) where T : BoardItem
        {
            var grid = new T[map2DArrayWidth, map2dArrayHeight];

            foreach (var boardItem in boardItems)
            {
                if (boardItem == null) continue;
                var coordinate = boardItem.Coordinate;
                grid[coordinate.X, coordinate.Z] = boardItem;
            }

            return grid;
        }

        public IReadOnlyDictionary<HexDirection, T?> GetNeighbours<T>(Coordinate axialCoordinate) where T : BoardItem
        {
            var minusX = axialCoordinate + HexDirection.MinusX;
            var plusX = axialCoordinate + HexDirection.PlusX;
            var minusXMinusZ = axialCoordinate + HexDirection.MinusXMinusZ;
            var minusZ = axialCoordinate + HexDirection.MinusZ;
            var plusZ = axialCoordinate + HexDirection.PlusZ;
            var plusXPlusZ = axialCoordinate + HexDirection.PlusXPlusZ;

            var toReturn = new Dictionary<HexDirection, T?>
            {
                [HexDirection.MinusX] = GetBoardItemWithDefault<T>(minusX.X, minusX.Z),
                [HexDirection.PlusX] = GetBoardItemWithDefault<T>(plusX.X, plusX.Z),
                [HexDirection.MinusXMinusZ] = GetBoardItemWithDefault<T>(minusXMinusZ.X, minusXMinusZ.Z),
                [HexDirection.MinusZ] = GetBoardItemWithDefault<T>(minusZ.X, minusZ.Z),
                [HexDirection.PlusZ] = GetBoardItemWithDefault<T>(plusZ.X, plusZ.Z),
                [HexDirection.PlusXPlusZ] = GetBoardItemWithDefault<T>(plusXPlusZ.X, plusXPlusZ.Z)
            };

            return toReturn;
        }

        public T? Get<T>(Coordinate axialCoordinate) where T : BoardItem => GetGridForType<T>()[axialCoordinate.X, axialCoordinate.Z];

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : BoardItem
        {
            GetGridForType<T>()[axialCoordinate.X, axialCoordinate.Z] = value;
        }

        public bool TryGet<T>(Coordinate axialCoordinate, out T? t) where T : BoardItem
        {
            t = GetBoardItemWithDefault<T>(axialCoordinate.X, axialCoordinate.Z);

            return t != null;
        }

        private T? GetBoardItemWithDefault<T>(int x, int z) where T : BoardItem
        {
            try
            {
                return GetGridForType<T>()[x, z];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        private T?[,] GetGridForType<T>() where T : BoardItem
        {
            if (_tileGrid is T[,] tileGrid)
                return tileGrid;
            if (_unitGrid is T[,] unitGrid)
                return unitGrid;


            throw new ArgumentOutOfRangeException($"{typeof(T).Name} is not implemented");
        }
    }
}