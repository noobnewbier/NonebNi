using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    [Serializable]
    public partial class EditorNode
    {
        [SerializeField] private TileData tileData;

        private List<EditorEntityData> _entityDatas;

        public EditorNode(TileData tileData)
        {
            this.tileData = tileData;
            _entityDatas = new List<EditorEntityData>();
        }

        public TileData TileData => tileData;

        public T? Get<T>() where T : EditorEntityData => _entityDatas.OfType<T>().FirstOrDefault();
        public bool Has<T>(T entityData) where T : EditorEntityData => _entityDatas.Any(e => e.Guid == entityData.Guid);

        public void Set<T>(T? entityData) where T : EditorEntityData
        {
            if (typeof(T) == typeof(EditorEntityData))
                Debug.LogWarning(
                    $"I am fairly certain you don't want to nuke everything in the {nameof(_entityDatas)}, future me are you sure this is what you want?"
                );

            _entityDatas.RemoveAll(e => e is T);
            if (entityData != null) _entityDatas.Add(entityData);
        }

        public Node ToNode()
        {
            return new Node(tileData, _entityDatas.Select(d => d.ToEntityData()).ToList());
        }
    }
}