using System;
using NonebNi.Core.Entities;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities
{
    //TODO: Something went wrong, why the fuck an editor only data is an EntityData?
    //I am thinking it's actually not that bad of an idea - what if TileEntityData is just obstacles that has the ability to modify the weight/other properties of a tile?
    [Obsolete("Something went terribly wrong here")]
    [Serializable]
    public class TileEntityData : EntityData
    {
        [SerializeField] private int weight;
        [SerializeField] private string name;

        public TileEntityData(Guid guid, string factionId, string name, int weight) : base(name, guid, factionId)
        {
            this.name = name;
            this.weight = weight;
        }

        public TileData CreateTileData() => new(name, weight);
    }
}