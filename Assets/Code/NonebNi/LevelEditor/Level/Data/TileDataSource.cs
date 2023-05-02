using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Data
{
    [CreateAssetMenu(menuName = "Data/TileData", fileName = "TileData")]
    public class TileDataSource : ScriptableObject
    {
        [field: SerializeField] public TileData TileData { get; private set; }
    }
}