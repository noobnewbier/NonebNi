using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.Core.Maps
{
    /// <summary>
    ///     The internal storage of <see cref="Map" />
    ///     A <see cref="Node" /> MUST have a <see cref="TileData" />, and may contains some <see cref="EntityData" />
    ///     Assumption:
    ///     For any type of <see cref="EntityData" />, a node can only hold at most 1 of it.
    /// </summary>
    [Serializable] //Unity doesn't support polymorphism + Serialization, we need to hand craft it
    public class Node
    {
        [SerializeField] private TileData tileData;

        [SerializeReference] private List<EntityData> entityDatas;

        public Node(TileData tileData)
        {
            this.tileData = tileData;
            entityDatas = new List<EntityData>();
        }

        public Node(TileData tileData, List<EntityData> entityDatas)
        {
            this.tileData = tileData;
            this.entityDatas = entityDatas;
        }

        public TileData TileData
        {
            get
            {
                var tileModifier = Get<TileModifierData>();

                return tileModifier?.TileData ?? tileData;
            }
        }

        public T? Get<T>() where T : EntityData => entityDatas.OfType<T>().FirstOrDefault();
        public bool Has<T>(T entityData) where T : EntityData => entityDatas.Contains(entityData);
        public bool Remove(EntityData entityData) => entityDatas.Remove(entityData);

        public void Set<T>(T? entityData) where T : EntityData
        {
            if (typeof(T) == typeof(EntityData))
                Debug.LogWarning(
                    $"I am fairly certain you don't want to nuke everything in the {nameof(entityDatas)}, future me are you sure this is what you want?"
                );

            entityDatas.RemoveAll(e => e is T);
            if (entityData != null) entityDatas.Add(entityData);
        }
    }
}