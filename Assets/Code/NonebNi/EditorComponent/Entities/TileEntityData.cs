using System;
using Code.NonebNi.Game.Entities;
using Code.NonebNi.Game.Tiles;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    [Serializable]
    public class TileEntityData : EntityData
    {
        //TODO: Something went wrong, why the fuck an editor only data is an EntityData?
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