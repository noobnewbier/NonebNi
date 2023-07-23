using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace NonebNi.Terrain
{
    /// <summary>
    ///     Class containing all data used to generate a mesh while triangulating a hex map.
    /// </summary>
    [Serializable]
    public class TerrainMeshData
    {
        // ReSharper disable once InconsistentNaming -- it cannot handle "s" after 2 and want to rename it with capital:) 
        [SerializeField] private List<Vector2> uv2s, uvs;
        [SerializeField] private List<Color> cellWeights;
        [SerializeField] private List<int> triangles;
        [SerializeField] private List<Vector3> vertices, cellIndices;
        private readonly Mesh _hexMesh;


        public TerrainMeshData()
        {
            _hexMesh = new Mesh
            {
                name = "Terrain Mesh"
            };
            uvs = ListPool<Vector2>.Get();
            uv2s = ListPool<Vector2>.Get();
            cellWeights = ListPool<Color>.Get();
            triangles = ListPool<int>.Get();
            vertices = ListPool<Vector3>.Get();
            cellIndices = ListPool<Vector3>.Get();
        }

        /// <summary>
        ///     Clear all data.
        /// </summary>
        public void Clear()
        {
            _hexMesh.Clear();
            vertices.Clear();
            cellWeights.Clear();
            cellIndices.Clear();
            uvs.Clear();
            uv2s.Clear();
            triangles.Clear();
        }

        /// <summary>
        ///     Apply all triangulation data to the underlying mesh.
        /// </summary>
        public Mesh Apply()
        {
            _hexMesh.SetVertices(vertices);

            _hexMesh.SetColors(cellWeights);
            _hexMesh.SetUVs(2, cellIndices);

            _hexMesh.SetUVs(0, uvs);
            _hexMesh.SetUVs(1, uv2s);

            _hexMesh.SetTriangles(triangles, 0);
            _hexMesh.RecalculateNormals();

            return _hexMesh;
        }

        /// <summary>
        ///     Add a triangle, applying perturbation to the positions.
        /// </summary>
        /// <param name="v1">First vertex position.</param>
        /// <param name="v2">Second vertex position.</param>
        /// <param name="v3">Third vertex position.</param>
        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = vertices.Count;
            //consider perturbing
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Add a triangle verbatim, without perturbing the positions.
        /// </summary>
        /// <param name="v1">First vertex position.</param>
        /// <param name="v2">Second vertex position.</param>
        /// <param name="v3">Third vertex position.</param>
        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        ///     Add UV coordinates for a triangle.
        /// </summary>
        /// <param name="uv1">First UV coordinates.</param>
        /// <param name="uv2">Second UV coordinates.</param>
        /// <param name="uv3">Third UV coordinates.</param>
        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
        }

        /// <summary>
        ///     Add UV2 coordinates for a triangle.
        /// </summary>
        /// <param name="uv1">First UV2 coordinates.</param>
        /// <param name="uv2">Second UV2 coordinates.</param>
        /// <param name="uv3">Third UV2 coordinates.</param>
        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
        }

        /// <summary>
        ///     Add cell data for a triangle.
        /// </summary>
        /// <param name="indices">Terrain type indices.</param>
        /// <param name="weights1">First terrain weights.</param>
        /// <param name="weights2">Second terrain weights.</param>
        /// <param name="weights3">Third terrain weights.</param>
        public void AddTriangleCellData(
            Vector3 indices,
            Color weights1,
            Color weights2,
            Color weights3
        )
        {
            cellIndices.Add(indices);
            cellIndices.Add(indices);
            cellIndices.Add(indices);
            cellWeights.Add(weights1);
            cellWeights.Add(weights2);
            cellWeights.Add(weights3);
        }

        /// <summary>
        ///     Add cell data for a triangle.
        /// </summary>
        /// <param name="indices">Terrain type indices.</param>
        /// <param name="weights">Terrain weights, uniform for entire triangle.</param>
        public void AddTriangleCellData(Vector3 indices, Color weights) =>
            AddTriangleCellData(indices, weights, weights, weights);

        /// <summary>
        ///     Add a quad, applying perturbation to the positions.
        /// </summary>
        /// <param name="v1">First vertex position.</param>
        /// <param name="v2">Second vertex position.</param>
        /// <param name="v3">Third vertex position.</param>
        /// <param name="v4">Fourth vertex position.</param>
        public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = vertices.Count;
            //consider perturbing
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        ///     Add a quad verbatim, without perturbing the positions.
        /// </summary>
        /// <param name="v1">First vertex position.</param>
        /// <param name="v2">Second vertex position.</param>
        /// <param name="v3">Third vertex position.</param>
        /// <param name="v4">Fourth vertex position.</param>
        public void AddQuadUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
        {
            var vertexIndex = vertices.Count;
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);
            vertices.Add(v4);
            triangles.Add(vertexIndex);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 1);
            triangles.Add(vertexIndex + 2);
            triangles.Add(vertexIndex + 3);
        }

        /// <summary>
        ///     Add UV coordinates for a quad.
        /// </summary>
        /// <param name="uv1">First UV coordinates.</param>
        /// <param name="uv2">Second UV coordinates.</param>
        /// <param name="uv3">Third UV coordinates.</param>
        /// <param name="uv4">Fourth UV coordinates.</param>
        public void AddQuadUV(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uvs.Add(uv1);
            uvs.Add(uv2);
            uvs.Add(uv3);
            uvs.Add(uv4);
        }

        /// <summary>
        ///     Add UV2 coordinates for a quad.
        /// </summary>
        /// <param name="uv1">First UV2 coordinates.</param>
        /// <param name="uv2">Second UV2 coordinates.</param>
        /// <param name="uv3">Third UV2 coordinates.</param>
        /// <param name="uv4">Fourth UV2 coordinates.</param>
        public void AddQuadUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3, Vector3 uv4)
        {
            uv2s.Add(uv1);
            uv2s.Add(uv2);
            uv2s.Add(uv3);
            uv2s.Add(uv4);
        }

        /// <summary>
        ///     Add UV coordinates for a quad.
        /// </summary>
        /// <param name="uMin">Minimum U coordinate.</param>
        /// <param name="uMax">Maximum U coordinate.</param>
        /// <param name="vMin">Minimum V coordinate.</param>
        /// <param name="vMax">Maximum V coordinate.</param>
        public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
        {
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMax, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
        }

        /// <summary>
        ///     Add UV2 coordinates for a quad.
        /// </summary>
        /// <param name="uMin">Minimum U2 coordinate.</param>
        /// <param name="uMax">Maximum U2 coordinate.</param>
        /// <param name="vMin">Minimum V2 coordinate.</param>
        /// <param name="vMax">Maximum V2 coordinate.</param>
        public void AddQuadUV2(float uMin, float uMax, float vMin, float vMax)
        {
            uv2s.Add(new Vector2(uMin, vMin));
            uv2s.Add(new Vector2(uMax, vMin));
            uv2s.Add(new Vector2(uMin, vMax));
            uv2s.Add(new Vector2(uMax, vMax));
        }

        /// <summary>
        ///     Add cell data for a quad.
        /// </summary>
        /// <param name="indices">Terrain type indices.</param>
        /// <param name="weights1">First terrain weights.</param>
        /// <param name="weights2">Second terrain weights.</param>
        /// <param name="weights3">Third terrain weights.</param>
        /// <param name="weights4">Fourth terrain weights.</param>
        public void AddQuadCellData(
            Vector3 indices,
            Color weights1,
            Color weights2,
            Color weights3,
            Color weights4
        )
        {
            cellIndices.Add(indices);
            cellIndices.Add(indices);
            cellIndices.Add(indices);
            cellIndices.Add(indices);
            cellWeights.Add(weights1);
            cellWeights.Add(weights2);
            cellWeights.Add(weights3);
            cellWeights.Add(weights4);
        }

        /// <summary>
        ///     Add cell data for a quad.
        /// </summary>
        /// <param name="indices">Terrain type indices.</param>
        /// <param name="weights1">First and second terrain weights, both the same.</param>
        /// <param name="weights2">Third and fourth terrain weights, both the same.</param>
        public void AddQuadCellData(Vector3 indices, Color weights1, Color weights2) =>
            AddQuadCellData(indices, weights1, weights1, weights2, weights2);

        /// <summary>
        ///     Add cell data for a quad.
        /// </summary>
        /// <param name="indices">Terrain type indices.</param>
        /// <param name="weights">Terrain weights, uniform for entire quad.</param>
        public void AddQuadCellData(Vector3 indices, Color weights) =>
            AddQuadCellData(indices, weights, weights, weights, weights);
    }
}