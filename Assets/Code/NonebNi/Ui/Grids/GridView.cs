using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Ui.Grids
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