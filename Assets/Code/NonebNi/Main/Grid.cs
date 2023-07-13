using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Main
{
    //todo: consider moving this to the Main asmdef
    public class Grid : MonoBehaviour
    {
        [SerializeField] private GridView gridView = null!;

        public void Init(ITerrainMeshCreator meshCreator)
        {
            gridView.Init(meshCreator);
        }
    }
}