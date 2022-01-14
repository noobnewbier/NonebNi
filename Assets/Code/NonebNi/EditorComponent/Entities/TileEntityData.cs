using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    //TODO: Something went wrong, why the fuck an editor only data is an EntityData?
    [Obsolete("Something went terribly wrong here")]
    [Serializable]
    public class TileEntityData : EntityData
    {
        [SerializeField] private float weight;
        [SerializeField] private string name;

        public TileEntityData(Guid guid, string name, float weight) : base(name, guid)
        {
            this.name = name;
            this.weight = weight;
        }

        public TileData CreateTileData() => new TileData(name, weight);
    }
}