using System;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataSource : EditorEntityDataSource<EditorEntityData<TileEntityData>>
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private float weight;

        public float Weight => weight;

        public override EditorEntityData<TileEntityData> CreateData(Guid guid) =>
            new EditorEntityData<TileEntityData>(guid, new TileEntityData(guid, tileName, weight));
    }
}