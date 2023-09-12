﻿using System;
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
        bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out TileData? tileData);
        TileData Get(Coordinate axialCoordinate);
        T? Get<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EntityData;
        bool Has<T>(Coordinate axialCoordinate) where T : EntityData;
        bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData;
        bool TryFind<T>(T entityData, out IEnumerable<Coordinate> coordinates) where T : EntityData;
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
        IEnumerable<UnitData> GetAllUnits();
    }

    public enum MoveResult
    {
        Success,
        ErrorTargetOccupied,
        ErrorNoEntityToBeMoved,
        NoEffect
    }

    public interface IMap : IReadOnlyMap
    {
        void Set(Coordinate axialCoordinate, TileData tileData);
        void Set<T>(Coordinate axialCoordinate, T? value) where T : EntityData;
        MoveResult Move<T>(T entity, Coordinate targetCoord) where T : EntityData;
        bool Remove<T>(T entityData) where T : EntityData;
        MoveResult Move<T>(Coordinate startCoord, Coordinate targetCoord) where T : EntityData;
        Coordinate Find<T>(T entityData) where T : EntityData;
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

            return storageCoord.X < width && storageCoord.Z < height && storageCoord.X >= 0 && storageCoord.Z >= 0;
        }

        public IEnumerable<UnitData> GetAllUnits()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery : Using linq will force us to use null-forgiving operator, which is worst than a foreach
            foreach (var unitData in nodes.Select(n => n.Get<UnitData>()))
                if (unitData != null)
                    yield return unitData;
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
                var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
                tileData = nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width)]
                    .TileData;
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

            return nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width)].TileData;
        }

        public void Set(Coordinate axialCoordinate, TileData tileData)
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width)].TileData
                .CopyValueFrom(tileData);
        }

        #endregion

        #region Entity

        public T? Get<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return nodes[storageCoordinate.X + storageCoordinate.Z * width].Get<T>();
        }

        public bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate);

            return t != null;
        }

        public bool Has<T>(Coordinate axialCoordinate) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            return GetBoardItemWithDefault<T>(storageCoordinate) != null;
        }

        public bool TryFind<T>(T entityData, out IEnumerable<Coordinate> coordinates) where T : EntityData
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

        public bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EntityData
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

        public Coordinate Find<T>(T entityData) where T : EntityData
        {
            if (!TryFind(entityData, out Coordinate coordinate))
            {
                throw new InvalidOperationException($"{entityData.Name} does not exist on the map!");
            }

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

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : EntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            nodes[StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(storageCoordinate, width)].Set(value);
        }

        public MoveResult Move<T>(T entity, Coordinate targetCoord) where T : EntityData
        {
            if (TryGet<T>(targetCoord, out var targetPosEntity))
                return entity == targetPosEntity ?
                    MoveResult.NoEffect : //Move to current pos does nothing 
                    MoveResult.ErrorTargetOccupied;

            if (!Remove(entity)) return MoveResult.ErrorNoEntityToBeMoved;

            Set(targetCoord, entity);
            return MoveResult.Success;
        }

        public MoveResult Move<T>(Coordinate startCoord, Coordinate targetCoord) where T : EntityData
        {
            if (!TryGet<T>(startCoord, out var target)) return MoveResult.ErrorNoEntityToBeMoved;

            return Move(target, targetCoord);
        }

        public bool Remove<T>(T entityData) where T : EntityData
        {
            if (TryFind(entityData, out Coordinate coord))
            {
                Set<T>(coord, null);

                return true;
            }

            return false;
        }

        #endregion
    }
}