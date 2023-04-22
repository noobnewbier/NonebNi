using System;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    /// The actual data to be placed in the map
    /// </summary>
    [Serializable]
    public struct TileData
    {
        public static readonly TileData Default = new TileData("Default", 1);

        [SerializeField] private int weight;
        [SerializeField] private string name;

        public TileData(string name, int weight)
        {
            this.name = name;
            this.weight = weight;
        }

        public string Name => name;

        public int Weight => weight;

        public void CopyValueFrom(TileData tileData)
        {
            weight = tileData.weight;
            name = tileData.name;
        }
    }
}