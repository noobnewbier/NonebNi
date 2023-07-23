using NonebNi.Terrain;
using NonebNi.Ui.Grids;
using UnityEngine;

namespace NonebNi.Main
{
    public class Terrain : MonoBehaviour
    {
        [SerializeField] private GridView gridView = null!;

        public void Init(ITerrainMeshCreator meshCreator)
        {
            gridView.Init(meshCreator);
        }
    }
}