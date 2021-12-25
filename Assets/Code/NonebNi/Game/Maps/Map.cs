using System;
using System.Collections.Generic;
using Code.NonebNi.Game.Coordinates;
using Code.NonebNi.Game.Entities;
using Code.NonebNi.Game.Tiles;
using UnityEngine;

namespace Code.NonebNi.Game.Maps
{
    public interface IReadOnlyMap
    {
        bool TryGet(Coordinate axialCoordinate, out TileData tileData);
        TileData Get(Coordinate axialCoordinate);
        T? Get<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryGet<T>(Coordinate axialCoordinate, out T t) where T : EntityData;
        bool Has<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData;
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
    }

    /// <summary>
    /// Storing weight of tiles and units positions.
    /// We need a way to validate the Map, so if for some reason(merging, user being an idiot) Map is not valid, we try our best to
    /// recover
    /// </summary>
    [Serializable]
    public class Map : IReadOnlyMap
    {
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private Node[] nodes;

        #region Init

        /// <summary>
        /// Create a map and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="Map" /> with no board items, where all tiles weight is set to 1</returns>
        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            nodes = new Node[this.width * this.height];

            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
                nodes[GetIndexFromStorageCoordinate(x, z)] = new Node(TileData.Default);
        }

        #endregion

        #region Tile specifics

        /*
         * As C# 8.0 doesn't support generics that can deal with both nullable reference and value type,
         * we can't share the helper method such as GetBoardItemWithDefaults here
         *
         * The best thing we can do without changing the TileData from a struct to class is to write the same code twice here
         * :sad face:
         */

        public bool TryGet(Coordinate axialCoordinate, out TileData tileData)
        {
            tileData = default;

            try
            {
                var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
                tileData = nodes[GetIndexFromStorageCoordinate(storageCoordinate)].TileData;
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public TileData Get(Coordinate axialCoordinate)
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            return nodes[GetIndexFromStorageCoordinate(storageCoordinate)].TileData;
        }

        public void Set(Coordinate axialCoordinate, TileData tileData)
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            nodes[GetIndexFromStorageCoordinate(storageCoordinate)].TileData.CopyValueFrom(tileData);
        }

        #endregion

        #region Entity

        public T? Get<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return nodes[storageCoordinate.X + storageCoordinate.Z * width].Get<T>();
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
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].Has(entityData))
                {
                    var storageCoordinate = StorageCoordinateFromIndex(i);
                    coordinate = storageCoordinate.ToAxial();
                    return true;
                }

            coordinate = default;
            return false;
        }

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : EntityData
        {
            try
            {
                return nodes[GetIndexFromStorageCoordinate(storageCoordinate)].Get<T>();
            }
            catch (IndexOutOfRangeException)
            {
                return default;
            }
        }

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            nodes[GetIndexFromStorageCoordinate(storageCoordinate)].Set(value);
        }

        public bool TryMoveEntityTo<T>(T entityData, Coordinate coordinate) where T : EntityData
        {
            if (TryFind(entityData, out var currentCoordinate))
                //todo: how do we deal with tile modifier?
                return true;

            return false;
        }

        #endregion

        #region Coordinates

        public IEnumerable<Coordinate> GetAllCoordinates()
        {
            Coordinate GetAxialCoordinateFromIndex(int xIndex, int zIndex)
            {
                var x = xIndex - zIndex / 2;
                return new Coordinate(x, zIndex);
            }

            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
                yield return GetAxialCoordinateFromIndex(x, z);
        }

        public bool IsCoordinateWithinMap(Coordinate coordinate)
        {
            var storageCoord = StorageCoordinate.FromAxial(coordinate);

            return storageCoord.X < width && storageCoord.Z < height && storageCoord.X >= 0 && storageCoord.Z >= 0;
        }

        private int GetIndexFromStorageCoordinate(StorageCoordinate storageCoordinate) =>
            GetIndexFromStorageCoordinate(storageCoordinate.X, storageCoordinate.Z);

        private int GetIndexFromStorageCoordinate(int x, int z) =>
            x + z * width;

        private StorageCoordinate StorageCoordinateFromIndex(int i)
        {
            var z = i / width;
            var x = i - z;

            return new StorageCoordinate(x, z);
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

            public Coordinate ToAxial() => ToAxial(X, Z);

            public static Coordinate ToAxial(int x, int z) => new Coordinate(Mathf.CeilToInt(x - z / 2), z);
        }

        #endregion
    }
}