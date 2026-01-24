using System;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities.Tile
{
    [CreateAssetMenu(menuName = "Data/TileModifier", fileName = "TileModifierData")]
    public class TileModifierDataSource : EditorEntityDataSource<EditorEntityData<TileModifierData>>
    {
        [SerializeField] private string modifierName = null!;
        [SerializeField] private TileDataSource tileDataSource = null!;
        [SerializeField] private bool isTileOccupier;

        public override EditorEntityData<TileModifierData> CreateData(Guid guid, string factionId) =>
            new(guid, new TileModifierData(modifierName, guid, factionId, tileDataSource.TileData, isTileOccupier));
    }
}