using System;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    /// <summary>
    ///     The actual data to be placed in the map
    /// </summary>
    [Serializable]
    public struct TileData
    {
        public const int ObstacleWeight = 9999;
        public static readonly TileData Default = new("Default", 1, false);

        [SerializeField] private int weight;
        [SerializeField] private string name;
        [field: SerializeField] public bool IsWall { get; private set; }

        public TileData(string name, int weight, bool isWall)
        {
            this.name = name;
            this.weight = weight;
            IsWall = isWall;
        }

        public string Name => name;

        public int Weight =>
            IsWall ?
                ObstacleWeight :
                weight;

        public void CopyValueFrom(TileData tileData)
        {
            weight = tileData.weight;
            name = tileData.name;
        }
    }
}