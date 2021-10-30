using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.Editors.Level.Entities.Tile
{
    [Serializable]
    public class TileEntityData : EntityData
    {
        [SerializeField] private float weight;
        [SerializeField] private string name;

        public TileEntityData(string name, float weight) : base(name)
        {
            this.name = name;
            this.weight = weight;
        }

        public TileData CreateTileData() => new TileData(name, weight);
    }
}