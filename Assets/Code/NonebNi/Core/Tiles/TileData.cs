using System;
using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    /// Todo: refactor such that tile is no longer an entity -> this seems inherently off... or is it?
    /// </summary>
    [Serializable]
    public class TileData : EntityData
    {
        public static readonly TileData Default = new TileData("Default", 1);


        [SerializeField] private float weight;

        public TileData(string name, float weight) : base(name)
        {
            this.weight = weight;
        }

        public float Weight => weight;
    }
}