using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.EditorComponent.Entities;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Maps
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
        public IReadOnlyList<EditorEntityData> AllEntities => _entityDatas;
        public EditorEntityData? CurrentOccupier => _entityDatas.FirstOrDefault(e => e.ToEntityData().IsTileOccupier);

        public T? Get<T>() where T : EditorEntityData => _entityDatas.OfType<T>().FirstOrDefault();
        public bool Has(EditorEntityData entityData) => _entityDatas.Any(e => e.Guid == entityData.Guid);

        public void Put(EditorEntityData entityData)
        {
            var data = entityData.ToEntityData();
            if (_entityDatas.Contains(entityData)) return;

            if (data.IsTileOccupier)
            {
                var existingTileOccupier = CurrentOccupier;
                if (existingTileOccupier != null)
                {
                    Log.Error(
                        $"Attempting to add {data.Name} into this node when another occupier already exist - this is invalid, operation ignored"
                    );
                    return;
                }
            }

            _entityDatas.Add(entityData);
        }

        public bool Remove(EditorEntityData entityData) => _entityDatas.Remove(entityData);

        public bool RemoveAll<T>() where T : EditorEntityData
        {
            return _entityDatas.RemoveAll(e => e is T) > 0;
        }

        public Node ToNode()
        {
            return new Node(tileData, _entityDatas.Select(d => d.ToEntityData()).ToList());
        }
    }
}