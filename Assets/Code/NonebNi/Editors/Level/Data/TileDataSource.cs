using System;
using Code.NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.Editors.Level.Data
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataSource : EditorEntityDataSource<EditorEntityData<TileEntityData>>
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private float weight;

        public float Weight => weight;

        public override EditorEntityData<TileEntityData> CreateData(Guid guid) =>
            new EditorEntityData<TileEntityData>(guid, new TileEntityData(tileName, weight));
    }
}