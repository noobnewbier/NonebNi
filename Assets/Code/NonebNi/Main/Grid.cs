using NonebNi.Main.Di;
using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Main
{
    //todo: consider moving this to the Main asmdef
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