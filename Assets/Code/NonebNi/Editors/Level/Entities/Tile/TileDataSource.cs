using UnityEngine;

namespace NonebNi.Editors.Level.Entities.Tile
{
    [CreateAssetMenu(menuName = "Data/Tile", fileName = "TileData")]
    public class TileDataSource : EntityDataSource<TileEntityData>
    {
        [SerializeField] private string tileName = null!;
        [SerializeField] private float weight;

        public float Weight => weight;
        public override TileEntityData CreateData() => new TileEntityData(tileName, weight);
    }
}