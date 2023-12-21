using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// The internal storage of <see cref="Map" />
    /// A <see cref="Node" /> MUST have a <see cref="TileData" />, and may contains some <see cref="EntityData" />
    ///
    /// Assumption:
    /// 1. For any type of <see cref="EntityData" />, a node can hold any amount of it.
    /// 2. At any node, at most 1 <see cref="EntityData"/> can be occupying it(i.e. <see cref="EntityData.IsTileOccupier"/>)
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

        public EntityData? CurrentOccupier => entityDatas.FirstOrDefault(e => e.IsTileOccupier);

        public T? Get<T>() where T : EntityData => entityDatas.OfType<T>().FirstOrDefault();
        public bool Has<T>(T entityData) where T : EntityData => entityDatas.Contains(entityData);
        public bool Remove(EntityData entityData) => entityDatas.Remove(entityData);

        public void Put(EntityData entityData)
        {
            if (entityDatas.Contains(entityData)) return;

            if (entityData.IsTileOccupier)
            {
                var existingTileOccupier = CurrentOccupier;
                if (existingTileOccupier != null)
                {
                    Log.Error(
                        $"Attempting to add {entityData.Name} into this node when another occupier already exist - this is invalid, operation ignored"
                    );
                    return;
                }
            }

            entityDatas.Add(entityData);
        }
    }
}