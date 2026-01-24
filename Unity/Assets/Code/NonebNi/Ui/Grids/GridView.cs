using NonebNi.Terrain;
using UnityEngine;

namespace NonebNi.Ui.Grids
{
    public class GridView : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter = null!;
        [SerializeField] private MeshCollider meshCollider = null!;


        public void Init(ITerrainMeshCreator terrainMeshCreator)
        {
            var mesh = terrainMeshCreator.Triangulate();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;
        }
    }
}