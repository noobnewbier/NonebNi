using NonebNi.Core.Entities;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataSource : EntityDataSource<TileData>
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private float weight;

        public float Weight => weight;
        public override TileData CreateData() => new TileData(tileName, weight);
    }
}