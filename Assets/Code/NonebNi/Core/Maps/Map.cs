using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEngine;

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

        /// <summary>
        /// Create a map and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="Map" /> with no board items, where all tiles weight is set to 1</returns>
        public Map(MapConfigData mapConfig) : this(
            Enumerable.Range(0, mapConfig.GetMap2DActualHeight() * mapConfig.GetMap2DActualWidth())
                      .Select(_ => new TileData("DEFAULT_NAME", 1)),
            mapConfig
        )
        {
        }


        private T[,] CreateGrid<T>(IEnumerable<T> boardItems) where T : EntityData
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

        public T? Get<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return GetGridForType<T>()[storageCoordinate.X, storageCoordinate.Z];
        }

        public bool TryGet<T>(Coordinate axialCoordinate, out T? t) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate);

            return t != null;
        }

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : EntityData
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

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            GetGridForType<T>()[storageCoordinate.X, storageCoordinate.Z] = value;
        }

        public bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData
        {
            var grid = GetGridForType<T>();

            for (var x = 0; x < grid.GetLength(0); x++)
            for (var z = 0; z < grid.GetLength(1); z++)
                if (grid[x, z] == entityData)
                {
                    coordinate = StorageCoordinate.ToAxial(x, z);
                    return true;
                }

            coordinate = default;
            return false;
        }

        private T?[,] GetGridForType<T>() where T : EntityData
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
        /// While they should be accessed through the axial coordinate(<seealso cref="Coordinate" />), we will convert them internally
        /// for both storage and accessing.
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

            public static Coordinate ToAxial(int x, int z) => new Coordinate(Mathf.CeilToInt(x - z / 2), z);
        }
    }
}