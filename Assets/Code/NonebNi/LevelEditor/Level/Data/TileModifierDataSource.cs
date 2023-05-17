using System;
using NonebNi.Core.Tiles;
using NonebNi.EditorComponent.Entities;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    [CreateAssetMenu(menuName = "Data/TileModifier", fileName = "TileModifierData")]
    public class TileModifierDataSource : EditorEntityDataSource<EditorEntityData<TileModifierData>>
    {
        [SerializeField] private string modifierName = null!;
        [SerializeField] private TileDataSource tileDataSource = null!;

        public override EditorEntityData<TileModifierData> CreateData(Guid guid, string factionId) =>
            new(guid, new TileModifierData(modifierName, guid, factionId, tileDataSource.TileData));
    }
}