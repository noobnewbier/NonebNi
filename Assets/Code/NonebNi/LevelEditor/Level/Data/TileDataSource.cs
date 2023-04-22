using System;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataSource : EditorEntityDataSource<EditorEntityData<TileEntityData>>
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private int weight;

        public int Weight => weight;

        public override EditorEntityData<TileEntityData> CreateData(Guid guid, string factionId) =>
            new EditorEntityData<TileEntityData>(guid, new TileEntityData(guid, factionId, tileName, weight));
    }
}