using NonebNi.Ui.Di;
using UnityEngine;

namespace NonebNi.Ui.Grids
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