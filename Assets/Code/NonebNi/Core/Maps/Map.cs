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
    public interface IReadOnlyMap
    {
        T? Get<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryGet<T>(Coordinate axialCoordinate, out T t) where T : EntityData;
        bool Has<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData;
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
    }

    /// <summary>
    /// Storing weight of tiles and units positions.
    /// </summary>
    [Serializable]
    public class Map : IReadOnlyMap
    {
        private readonly int _height;
        private readonly TileData?[] _tileDatas;
        private readonly UnitData?[] _unitDatas;
        private readonly int _width;

        public Map(IEnumerable<TileData> tiles,
                   IEnumerable<UnitData> units,
                   int width,
                   int height)
        {
            _width = width;
            _height = height;


            T[] CreateArray<T>(IEnumerable<T> boardItems) where T : EntityData
            {
                var datas = new T[_width * _height];

                foreach (var (data, i) in boardItems.Select((data, i) => (data, i)))
                {
                    datas[i] = data;
                }

                return datas;
            }

            _tileDatas = CreateArray(tiles);
            _unitDatas = CreateArray(units);
        }

        /// <summary>
        /// Create a map and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="Map" /> with no board items, where all tiles weight is set to 1</returns>
        public Map(int width, int height) : this(
            Enumerable.Range(0, width * height).Select(_ => new TileData("DEFAULT_NAME", 1)),
            new List<UnitData>(),
            width,
            height
        )
        {
        }

        public T? Get<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return GetDatasForType<T>()[storageCoordinate.X + storageCoordinate.Z * _width];
        }

        public bool TryGet<T>(Coordinate axialCoordinate, out T t) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate)!;

            // no, t can be null here, it's just without some MaybeNullWhenAttribute(as unity doesn't support .net 2.1)
            // there is no clear way to express my intent here. User should be checking the bool value anyway,
            // so I think we are safe here.
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            return t != null;
        }

        public bool Has<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            return GetBoardItemWithDefault<T>(storageCoordinate) != null;
        }

        public bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData
        {
            var grid = GetDatasForType<T>();

            for (var x = 0; x < grid.GetLength(0); x++)
            for (var z = 0; z < grid.GetLength(1); z++)
                if (grid[x + z * _width] == entityData)
                {
                    coordinate = StorageCoordinate.ToAxial(x, z);
                    return true;
                }

            coordinate = default;
            return false;
        }

        public IEnumerable<Coordinate> GetAllCoordinates()
        {
            Coordinate GetAxialCoordinateFromIndex(int xIndex, int zIndex)
            {
                var x = xIndex - zIndex / 2;
                return new Coordinate(x, zIndex);
            }

            for (var x = 0; x < _width; x++)
            for (var z = 0; z < _height; z++)
                yield return GetAxialCoordinateFromIndex(x, z);
        }

        public bool IsCoordinateWithinMap(Coordinate coordinate)
        {
            var storageCoord = StorageCoordinate.FromAxial(coordinate);

            return storageCoord.X < _width && storageCoord.Z < _height && storageCoord.X >= 0 && storageCoord.Z >= 0;
        }

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : EntityData
        {
            try
            {
                return GetDatasForType<T>()[storageCoordinate.X + storageCoordinate.Z * _width];
            }
            catch (IndexOutOfRangeException)
            {
                return null;
            }
        }

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            GetDatasForType<T>()[storageCoordinate.X + storageCoordinate.Z * _width] = value;
        }

        private T?[] GetDatasForType<T>() where T : EntityData
        {
            if (_tileDatas is T[] tiles)
                return tiles;
            if (_unitDatas is T[] units)
                return units;

            throw new ArgumentOutOfRangeException($"{typeof(T).Name} is not implemented");
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