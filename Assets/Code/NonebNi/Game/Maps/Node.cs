using System;
using System.Collections.Generic;
using System.Linq;
using Code.NonebNi.Game.Entities;
using Code.NonebNi.Game.Tiles;
using UnityEngine;

namespace Code.NonebNi.Game.Maps
{
    /// <summary>
    /// The internal storage of <see cref="Map" />
    /// A <see cref="Node" /> MUST have a <see cref="TileData" />, and may contains some <see cref="EntityData" />
    /// Assumption:
    /// For any type of <see cref="EntityData" />, a node can only hold at most 1 of it.
    /// </summary>
    [Serializable] //Unity doesn't support polymorphism + Serialization, we need to hand craft it
    public class Node
    {
        [SerializeField] private TileData tileData;

        private List<EntityData> _entityDatas;

        public Node(TileData tileData)
        {
            this.tileData = tileData;
            _entityDatas = new List<EntityData>();
        }

        public TileData TileData => tileData;

        public T? Get<T>() where T : EntityData => _entityDatas.OfType<T>().FirstOrDefault();
        public bool Has<T>(T entityData) where T : EntityData => _entityDatas.Contains(entityData);

        public void Set<T>(T? entityData) where T : EntityData
        {
            if (typeof(T) == typeof(EntityData))
                Debug.LogWarning(
                    $"I am fairly certain you don't want to nuke everything in the {nameof(_entityDatas)}, future me are you sure this is what you want?"
                );

            _entityDatas.RemoveAll(e => e is T);
            if (entityData != null) _entityDatas.Add(entityData);
        }
    }
}