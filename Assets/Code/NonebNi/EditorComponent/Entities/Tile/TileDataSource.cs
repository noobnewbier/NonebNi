using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities.Tile
{
    [CreateAssetMenu(menuName = "Data/TileData", fileName = "TileData")]
    public class TileDataSource : ScriptableObject
    {
        [field: SerializeField] public TileData TileData { get; private set; }
    }
}