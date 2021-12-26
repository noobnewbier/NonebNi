using NonebNi.Game.Di;
using UnityEngine;

namespace NonebNi.Game.Grids
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private GridView gridView = null!;

        public void Init(IGridComponent gridComponent)
        {
            var levelData = gridComponent.GetLevelData();
            gridView.Init(gridComponent.CoordinateAndPositionService, levelData.Map, levelData.WorldConfig);
        }
    }
}