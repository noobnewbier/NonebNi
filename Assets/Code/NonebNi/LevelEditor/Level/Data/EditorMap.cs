﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    public interface IEditorMap
    {
        bool TryGet(Coordinate axialCoordinate, out TileData tileData);
        TileData Get(Coordinate axialCoordinate);
        T? Get<T>(Coordinate axialCoordinate) where T : EditorEntityData;
        bool TryGet<T>(Coordinate axialCoordinate, out T? t) where T : EditorEntityData;
        bool Has<T>(Coordinate axialCoordinate) where T : EditorEntityData;
        bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EditorEntityData;
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
        void Set(Coordinate axialCoordinate, TileData tileData);
        void Set<T>(Coordinate axialCoordinate, T? value) where T : EditorEntityData;
        Map ToMap();
    }

    /// <summary>
    /// Editor version of <see cref="Map" />
    /// It consist of basically copy-pasted code from Map, except we are using EditorNodes here. The main reason for this WET thing
    /// is that I really want to avoid changing
    /// implementation of the gameplay code because of the editor(with the fundamental idea that the editor-version should augments
    /// the game-version data)
    /// I'm not sure how best to handle this yet, we will see how this goes
    /// </summary>
    [Serializable]
    public class EditorMap : IEditorMap
    {
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private EditorNode[] nodes;

        #region Init

        /// <summary>
        /// Create a editorMap and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
        /// </summary>
        /// <returns>An empty <see cref="EditorMap" /> with no board items, where all tiles weight is set to 1</returns>
        public EditorMap(int width, int height)
        {
            this.width = width;
            this.height = height;
            nodes = new EditorNode[this.width * this.height];

            for (var x = 0; x < width; x++)
            for (var z = 0; z < height; z++)
                nodes[GetIndexFromStorageCoordinate(x, z)] = new EditorNode(TileData.Default);
        }

        #endregion

        public Map ToMap()
        {
            return new Map(width, height, nodes.Select(n => n.ToNode()).ToArray());
        }

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

        #region EditorEntity

        public T? Get<T>(Coordinate axialCoordinate) where T : EditorEntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            return nodes[storageCoordinate.X + storageCoordinate.Z * width].Get<T>();
        }

        public bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EditorEntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            t = GetBoardItemWithDefault<T>(storageCoordinate);

            return t != null;
        }

        public bool Has<T>(Coordinate axialCoordinate) where T : EditorEntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);

            return GetBoardItemWithDefault<T>(storageCoordinate) != null;
        }

        public bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EditorEntityData
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

        private T? GetBoardItemWithDefault<T>(StorageCoordinate storageCoordinate) where T : EditorEntityData
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

        public void Set<T>(Coordinate axialCoordinate, T? value) where T : EditorEntityData
        {
            var storageCoordinate = StorageCoordinate.FromAxial(axialCoordinate);
            nodes[GetIndexFromStorageCoordinate(storageCoordinate)].Set(value);
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
            StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(x, z, width);

        private StorageCoordinate StorageCoordinateFromIndex(int i)
        {
            return StorageCoordinate.StorageCoordinateFromIndex(i, width);
        }

        #endregion
    }
}