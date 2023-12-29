using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    ///     While the underlying tile/coordinate should have a "base" <see cref="TileData" />,
    ///     <see cref="TileModifierData" /> overrides whatever that's originally stored in the tile.
    ///     This is useful for breakable/dynamic objects, and some nice to have features such as tooltip on hover as this is still
    ///     at heart an <see cref="EntityData" />
    /// </summary>
    [Serializable]
    public class TileModifierData : EntityData
    {
        [field: SerializeField] private bool isTileOccupier;
        [field: SerializeField] public TileData TileData { get; private set; }

        public TileModifierData(
            string name,
            Guid serializableGuid,
            string factionId,
            TileData tileData,
            bool isTileOccupier) : base(
            name,
            serializableGuid,
            factionId
        )
        {
            TileData = tileData;
            this.isTileOccupier = isTileOccupier;
        }

        public override bool IsTileOccupier => isTileOccupier;
    }
}