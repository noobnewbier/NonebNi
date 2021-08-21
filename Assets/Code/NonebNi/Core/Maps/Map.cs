using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.BoardItems;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// Storing weight of tiles and units positions.
    /// </summary>
    public class Map
    {
        private readonly MapConfigData _mapConfig;
        private readonly TileData?[,] _tileGrid;
        private readonly UnitData?[,] _unitGrid;

        public Map(IEnumerable<TileData> tiles, IEnumerable<UnitData> units, MapConfigData mapConfig)
        {
            _mapConfig = mapConfig;
            _tileGrid = CreateGrid(tiles);
            _unitGrid = CreateGrid(units);
        }

        public Map(IEnumerable<TileData> tiles, MapConfigData mapConfig)
        {
            _mapConfig = mapConfig;
            _tileGrid = CreateGrid(tiles);
            _unitGrid = CreateGrid(new List<UnitData>());
        }


        private T[,] CreateGrid<T>(IEnumerable<T> boardItems) where T : BoardItemData
        {
            var mapWidth = _mapConfig.GetMap2DArrayWidth();
            var mapHeight = _mapConfig.GetMap2DArrayHeight();
            var grid = new T[mapWidth, mapHeight];

            foreach (var (data, i) in boardItems.Select((data, i) => (data, i)))
            {
                if (data == null) continue;

                var z = i / mapWidth;
                var x = i - z * mapWidth;

                var coordinate = new StorageCoordinate(x, z);
                grid[coordinate.X, coordinate.Z] = data;
            }

            return grid;
        }

        public IReadOnlyDictionary<HexDirection, T?> GetNeighbours<T>(Coordinate axialCoordinate) where T : BoardItemData
        {
            var minusX = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.MinusX);
            var plusX = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.PlusX);
            var minusXMinusZ = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.MinusXMinusZ);
            var minusZ = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.MinusZ);
            var plusZ = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.PlusZ);
            var plusXPlusZ = StorageCoordinate.FromAxial(axialCoordinate + HexDirection.PlusXPlusZ);

            var toReturn = new Dictionary<HexDirection, T?>
            {
                [HexDirection.MinusX] = GetBoardItemWithDefault<T>(minusX),
                [HexDirection.PlusX] = GetBoardItemWithDefault<T>(plusX),
                [HexDirection.MinusXMinusZ] = GetBoardItemWithDefault<T>(minusXMinusZ),
                [HexDirection.MinusZ] = GetBoardItemWithDefault<T>(minusZ),
                [HexDirection.PlusZ] = GetBoardItemWithDefault<T>(plusZ),
                [HexDirection.PlusXPlusZ] = GetBoardItemWithDefault<T>(plusXPlusZ)
            };

            return toReturn;
        }

        public T? Get<T>(Coordinate axialCoordinate) where T : BoardItemData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return GetGridForType<T>()[storageCoordinate.X, storageCoordinate.Z];
        }

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : BoardItemData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            GetGridForType<T>()[storageCoordinate.X, storageCoordinate.Z] = value;
        }

        public bool TryGet<T>(Coordinate axialCoordinate, out T? t) where T : BoardItemData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate);

            return t != null;
        }

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : BoardItemData
        {
            try
            {
                return GetGridForType<T>()[storageCoordinate.X, storageCoordinate.Z];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public T?[,] GetGridForType<T>() where T : BoardItemData
        {
            if (_tileGrid is T[,] tileGrid)
                return tileGrid;
            if (_unitGrid is T[,] unitGrid)
                return unitGrid;

            throw new ArgumentOutOfRangeException($"{typeof(T).Name} is not implemented");
        }

        public IEnumerable<Coordinate> GetAllCoordinates()
        {
            Coordinate GetAxialCoordinateFromIndex(int xIndex, int zIndex)
            {
                var z = zIndex;
                var x = xIndex - z / 2;
                return new Coordinate(x, z);
            }

            for (var x = 0; x < _mapConfig.GetMap2DActualWidth(); x++)
            for (var z = 0; z < _mapConfig.GetMap2DActualHeight(); z++)
                yield return GetAxialCoordinateFromIndex(x, z);
        }

        /// <summary>
        /// The data is stored in the <see cref="StorageCoordinate" />, which is just the x,z index in the 2d array.
        /// While they should be accessed through the axial coordinate(<seealso cref="Coordinate" />), we will convert them internally for both storage and
        /// accessing.
        /// </summary>
        private readonly struct StorageCoordinate
        {
            public readonly int X;
            public readonly int Z;

            public StorageCoordinate(int x, int z)
            {
                X = x;
                Z = z;
            }

            public static StorageCoordinate FromAxial(Coordinate coordinate)
            {
                var z = coordinate.Z;
                var x = coordinate.X + z / 2;
                return new StorageCoordinate(x, z);
            }
        }
    }
}