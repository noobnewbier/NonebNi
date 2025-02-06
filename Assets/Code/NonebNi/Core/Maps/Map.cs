using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        bool IsOccupied(Coordinate axialCoordinate);
        bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out TileData? tileData);
        TileData Get(Coordinate axialCoordinate);
        T? Get<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EntityData;
        bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out IEnumerable<EntityData>? datas);
        bool Has<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryFind(EntityData entityData, out Coordinate coordinate);
        bool TryFind(EntityData entityData, out IEnumerable<Coordinate> coordinates);
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
        IEnumerable<UnitData> GetAllUnits();
    }

    public enum MoveResult
    {
        Success,
        ErrorTargetOccupied,
        ErrorEntityIsNotOnBoard,
        NoEffect
    }

    public interface IMap : IReadOnlyMap
    {
        void Set(Coordinate axialCoordinate, TileData tileData);
        MoveResult Move(EntityData entity, Coordinate targetCoord);
        void Put(Coordinate axialCoordinate, EntityData value);
        bool Remove<T>(T entityData) where T : EntityData;
        MoveResult Move<T>(Coordinate startCoord, Coordinate targetCoord) where T : EntityData;
        Coordinate Find(EntityData entityData);
    }

    /// <summary>
    ///     Storing weight of tiles and units positions.
    ///     We need a way to validate the Map, so if for some reason(merging, user being an idiot) Map is not valid,
    ///     we try our best to recover
    /// </summary>
    [Serializable]
    public class Map : IMap
    {
        [SerializeField] private int height;
        [SerializeField] private int width;

        /// <summary>
        ///     The data is stored in the <see cref="StorageCoordinate" />, which is just the x,z index in a jagged array,
        ///     which we further flatten the array into a single row for efficiency.
        ///     While they should be accessed from the public API through the axial coordinate(<seealso cref="Coordinate" />),
        ///     we will convert them internally for both storage and accessing.
        /// </summary>
        [SerializeField] private Node[] nodes;

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

            return storageCoord.X < width && storageCoord.Z < height && storageCoord is { X: >= 0, Z: >= 0 };
        }

        public IEnumerable<UnitData> GetAllUnits()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery : Using linq will force us to use null-forgiving operator, which is worst than a foreach
            foreach (var unitData in nodes.Select(n => n.Get<UnitData>()))
                if (unitData != null)
                    yield return unitData;
        }

        private Node GetNodeFromAxialCoordinate(Coordinate coordinate)
        {
            if (!TryGetNodeFromAxialCoordinate(coordinate, out var node))
                throw new ArgumentOutOfRangeException(nameof(coordinate), "is out of range!");

            return node;
        }

        private bool TryGetNodeFromAxialCoordinate(Coordinate coordinate, [NotNullWhen(true)] out Node? output)
        {
            var storageCoordinate = StorageCoordinate.FromAxial(coordinate);
            var index = StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width);
            if (index >= 0 && index < nodes.Length)
            {
                output = nodes[index];
                return true;
            }

            output = default;
            return false;
        }

        #region Init

        /// <summary>
        ///     Create a map and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="Map" /> with no board items, where all tiles weight is set to 1</returns>
        public Map(int width, int height)
        {
            this.width = width;
            this.height = height;
            nodes = new Node[this.width * this.height];

            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
                nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(x, z, width)] = new Node(TileData.Default);
        }

        public Map(int width, int height, Node[] nodes)
        {
            if (nodes.Length != height * width)
                throw new ArgumentException(
                    $"{nameof(nodes)}'s length is invalid, expected to be {width * height} but is {nodes.Length}"
                );

            this.height = height;
            this.width = width;
            this.nodes = nodes;
        }

        #endregion

        #region Tile specifics

        public bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out TileData? tileData)
        {
            tileData = default;

            try
            {
                var node = GetNodeFromAxialCoordinate(axialCoordinate);
                tileData = node.TileData;

                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }

        public TileData Get(Coordinate axialCoordinate)
        {
            var node = GetNodeFromAxialCoordinate(axialCoordinate);
            return node.TileData;
        }

        public void Set(Coordinate axialCoordinate, TileData tileData)
        {
            var node = GetNodeFromAxialCoordinate(axialCoordinate);
            node.TileData.CopyValueFrom(tileData);
        }

        #endregion

        #region Entity

        public bool IsOccupied(Coordinate axialCoordinate)
        {
            var node = GetNodeFromAxialCoordinate(axialCoordinate);

            return node.CurrentOccupier != null;
        }

        public T? Get<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var node = GetNodeFromAxialCoordinate(axialCoordinate);
            return node.Get<T>();
        }

        public bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate);

            return t != null;
        }

        public bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out IEnumerable<EntityData>? datas)
        {
            if (TryGetNodeFromAxialCoordinate(axialCoordinate, out var node))
            {
                datas = node.AllEntities;
                return true;
            }

            datas = default;
            return false;
        }

        public bool Has<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            return GetBoardItemWithDefault<T>(storageCoordinate) != null;
        }

        public bool TryFind(EntityData entityData, out IEnumerable<Coordinate> coordinates)
        {
            var toReturn = new List<Coordinate>();
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].Has(entityData))
                {
                    var storageCoordinate = StorageCoordinate.StorageCoordinateFromIndex(i, width);
                    toReturn.Add(storageCoordinate.ToAxial());
                }

            coordinates = toReturn;
            return coordinates.Any();
        }

        public bool TryFind(EntityData entityData, out Coordinate coordinate)
        {
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].Has(entityData))
                {
                    var storageCoordinate = StorageCoordinate.StorageCoordinateFromIndex(i, width);
                    coordinate = storageCoordinate.ToAxial();
                    return true;
                }

            coordinate = default;
            return false;
        }

        public Coordinate Find(EntityData entityData)
        {
            if (!TryFind(entityData, out Coordinate coordinate))
                throw new InvalidOperationException($"{entityData.Name} does not exist on the map!");

            return coordinate;
        }

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : EntityData
        {
            try
            {
                return nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width)].Get<T>();
            }
            catch (IndexOutOfRangeException)
            {
                return default;
            }
        }

        public void Put(Coordinate axialCoordinate, EntityData value)
        {
            var node = GetNodeFromAxialCoordinate(axialCoordinate);
            node.Put(value);
        }

        public MoveResult Move(EntityData entity, Coordinate targetCoord)
        {
            if (!TryFind(entity, out Coordinate currentCoord)) return MoveResult.ErrorEntityIsNotOnBoard;
            if (currentCoord == targetCoord) return MoveResult.NoEffect;
            if (IsOccupied(targetCoord)) return MoveResult.ErrorTargetOccupied;

            Remove(entity);
            Put(targetCoord, entity);

            return MoveResult.Success;
        }

        public MoveResult Move<T>(Coordinate startCoord, Coordinate targetCoord) where T : EntityData
        {
            if (!TryGet<T>(startCoord, out var target)) return MoveResult.ErrorEntityIsNotOnBoard;

            return Move(target, targetCoord);
        }

        public bool Remove<T>(T entityData) where T : EntityData
        {
            var isRemoved = false;
            foreach (var node in nodes)
            {
                if (!node.Remove(entityData)) continue;

                isRemoved = true;
            }

            return isRemoved;
        }

        #endregion
    }
}