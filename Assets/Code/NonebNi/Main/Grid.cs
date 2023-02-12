using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Main
{
    //todo: consider moving this to the Main asmdef
    public class Grid : MonoBehaviour
    {
        [SerializeField] private GridView gridView = null!;

        public void Init(ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map,
            WorldConfigData worldConfig)
        {
            gridView.Init(coordinateAndPositionService, map, worldConfig);
        }
    }
}