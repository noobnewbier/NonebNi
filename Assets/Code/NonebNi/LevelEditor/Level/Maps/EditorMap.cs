using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Maps
{
    public interface IReadonlyEditorMap
    {
        bool TryGet(Coordinate axialCoordinate, out TileData tileData);
        bool TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : EditorEntityData;
        TileData Get(Coordinate axialCoordinate);
        T? Get<T>(Coordinate axialCoordinate) where T : EditorEntityData;
        bool Has<T>(Coordinate axialCoordinate) where T : EditorEntityData;
        bool TryFind<T>(T entityData, out Coordinate coordinate) where T : EditorEntityData;
        bool TryFind<T>(T entityData, out IEnumerable<Coordinate> coordinates) where T : EditorEntityData;
        IEnumerable<Coordinate> GetAllCoordinates();
        bool IsCoordinateWithinMap(Coordinate coordinate);
    }

    public interface IEditorMap : IReadonlyEditorMap
    {
        bool Remove(EditorEntityData entityData);
        void Set(Coordinate axialCoordinate, TileData tileData);
        void Set<T>(Coordinate axialCoordinate, T? value) where T : EditorEntityData;
        Map ToMap();
    }

    /// <summary>
    ///     Editor version of <see cref="Map" />
    ///     It consist of basically copy-pasted code from Map, except we are using EditorNodes here.
    ///     The main reason for this WET thing is that I really want to avoid changing
    ///     implementation of the gameplay code because of the editor
    ///     (with the fundamental idea that the editor-version should augments the game-version data)
    ///     I'm not sure how best to handle this yet, we will see how this goes
    /// </summary>
    [Serializable]
    public class EditorMap : IEditorMap, IReadOnlyMap
    {
        [SerializeField] private int height;
        [SerializeField] private int width;
        [SerializeField] private EditorNode[] nodes;

        #region Init

        /// <summary>
        ///     Create a editorMap and fill with tiles of weight 1 with the given <see cref="MapConfigScriptable" />
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
                var node = GetNodeFromCoordinate(axialCoordinate);
                tileData = node.TileData;
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        public TileData Get(Coordinate axialCoordinate)
        {
            var node = GetNodeFromCoordinate(axialCoordinate);

            return node.TileData;
        }

        public void Set(Coordinate axialCoordinate, TileData tileData)
        {
            var node = GetNodeFromCoordinate(axialCoordinate);

            node.TileData.CopyValueFrom(tileData);
        }

        #endregion

        #region IReadonlyMap

        public IEnumerable<UnitData> GetAllUnits()
        {
            // ReSharper disable once LoopCanBeConvertedToQuery : Using linq will force us to use null-forgiving operator, which is worst than a foreach
            foreach (var unitData in nodes.Select(n => n.Get<EditorEntityData<UnitData>>()))
                if (unitData != null)
                    yield return unitData.ToTypedEntityData();
        }

        public bool IsOccupied(Coordinate axialCoordinate)
        {
            var node = GetNodeFromCoordinate(axialCoordinate);

            return node.CurrentOccupier != null;
        }

        bool IReadOnlyMap.TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out TileData? tileData)
        {
            var result = TryGet(axialCoordinate, out TileData data);
            tileData = data;

            return result;
        }

        T? IReadOnlyMap.Get<T>(Coordinate axialCoordinate) where T : class
        {
            var editorEntityData = Get<EditorEntityData<T>>(axialCoordinate);
            return editorEntityData?.ToTypedEntityData();
        }

        bool IReadOnlyMap.TryGet<T>(Coordinate axialCoordinate, [NotNullWhen(true)] out T? t) where T : class
        {
            var result = TryGet<EditorEntityData<T>>(axialCoordinate, out var data);
            t = data?.ToTypedEntityData();

            return result;
        }

        public bool TryGet(Coordinate axialCoordinate, [NotNullWhen(true)] out IEnumerable<EntityData>? datas)
        {
            if (!IsCoordinateWithinMap(axialCoordinate))
            {
                datas = default;
                return false;
            }

            var node = GetNodeFromCoordinate(axialCoordinate);
            datas = node.AllEntities.Select(e => e.ToEntityData());
            return false;
        }

        bool IReadOnlyMap.Has<T>(Coordinate axialCoordinate) => Has<EditorEntityData<T>>(axialCoordinate);

        bool IReadOnlyMap.TryFind(EntityData entityData, out Coordinate coordinate)
        {
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].ToNode().Has(entityData))
                {
                    var storageCoordinate = StorageCoordinateFromIndex(i);
                    coordinate = storageCoordinate.ToAxial();
                    return true;
                }

            coordinate = default;
            return false;
        }

        bool IReadOnlyMap.TryFind(EntityData entityData, out IEnumerable<Coordinate> coordinates)
        {
            var toReturn = new List<Coordinate>();
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].ToNode().Has(entityData))
                {
                    var storageCoordinate = StorageCoordinateFromIndex(i);
                    toReturn.Add(storageCoordinate.ToAxial());
                }

            coordinates = toReturn;
            return coordinates.Any();
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

        public bool TryFind<T>(T entityData, out IEnumerable<Coordinate> coordinates) where T : EditorEntityData
        {
            var toReturn = new List<Coordinate>();
            for (var i = 0; i < nodes.Length; i++)
                if (nodes[i].Has(entityData))
                {
                    var storageCoordinate = StorageCoordinateFromIndex(i);
                    toReturn.Add(storageCoordinate.ToAxial());
                }

            coordinates = toReturn;
            return coordinates.Any();
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
            var node = GetNodeFromCoordinate(axialCoordinate);

            node.RemoveAll<T>();
            if (value != null) node.Put(value);
        }

        public bool Remove(EditorEntityData entityData)
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

        private EditorNode GetNodeFromCoordinate(Coordinate coordinate)
        {
            var storageCoordinate = StorageCoordinate.FromAxial(coordinate);
            var index = GetIndexFromStorageCoordinate(storageCoordinate);

            return nodes[index];
        }

        private int GetIndexFromStorageCoordinate(StorageCoordinate storageCoordinate) =>
            GetIndexFromStorageCoordinate(storageCoordinate.X, storageCoordinate.Z);

        private int GetIndexFromStorageCoordinate(int x, int z) =>
            StorageCoordinate.Get1DArrayIndexFromStorageCoordinate(x, z, width);

        private StorageCoordinate StorageCoordinateFromIndex(int i) =>
            StorageCoordinate.StorageCoordinateFromIndex(i, width);

        #endregion
    }
}