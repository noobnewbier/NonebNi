using Noneb.Core.Game.Common.BoardItems;
using UnityEngine;

namespace Noneb.Core.Game.Tiles
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataScriptable : BoardItemDataScriptable
    {
        [SerializeField] private string tileName;
        [SerializeField] private float weight;

        public float Weight => weight;

        public TileData ToData() => new TileData(Icon, tileName, this);
    }
}