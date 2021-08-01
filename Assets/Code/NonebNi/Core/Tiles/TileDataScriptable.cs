using NonebNi.Core.BoardItems;
using UnityEngine;

namespace NonebNi.Core.Tiles
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataScriptable : BoardItemDataScriptable
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private float weight;

        public float Weight => weight;

        public TileData ToData() => new TileData(tileName, weight);
    }
}