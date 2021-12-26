using NonebNi.Game.Level;
using NonebNi.Game.Maps;
using UnityEngine;

namespace NonebNi.Game.Grids
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private TileView tileViewPrefab = null!;

        public void Init(CoordinateAndPositionService coordinateAndPositionService, Map map, WorldConfigData worldConfigData)
        {
            foreach (var coordinate in map.GetAllCoordinates())
            {
                var pos = coordinateAndPositionService.FindPosition(coordinate);

                var tile = Instantiate(tileViewPrefab, transform);
                tile.Init(pos, worldConfigData);
            }
        }
    }
}