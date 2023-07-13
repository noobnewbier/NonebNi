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
    public class
        TerrainMeshData //TODO: we can make it pure data? and leave the mesh editing func outside.(apply, sharedmesh assignment etc)
    {
        private readonly Mesh _hexMesh;
        private List<Color> _cellWeights;
        private List<int> _triangles;

        // ReSharper disable once InconsistentNaming - jesus it just can't deal with the fact that uv2 is one word.
        private List<Vector2> _uv2s;

        private List<Vector2> _uvs;
        private List<Vector3> _vertices, _cellIndices;

        public TerrainMeshData()
        {
            _hexMesh = new Mesh
            {
                name = "Terrain Mesh"
            };
            _uvs = ListPool<Vector2>.Get();
            _uv2s = ListPool<Vector2>.Get();
            _cellWeights = ListPool<Color>.Get();
            _triangles = ListPool<int>.Get();
            _vertices = ListPool<Vector3>.Get();
            _cellIndices = ListPool<Vector3>.Get();
        }

        /// <summary>
        ///     Clear all data.
        /// </summary>
        public void Clear()
        {
            _hexMesh.Clear();
            _vertices.Clear();
            _cellWeights.Clear();
            _cellIndices.Clear();
            _uvs.Clear();
            _uv2s.Clear();
            _triangles.Clear();
        }

        /// <summary>
        ///     Apply all triangulation data to the underlying mesh.
        /// </summary>
        public Mesh Apply()
        {
            _hexMesh.SetVertices(_vertices);

            _hexMesh.SetColors(_cellWeights);
            _hexMesh.SetUVs(2, _cellIndices);

            _hexMesh.SetUVs(0, _uvs);
            _hexMesh.SetUVs(1, _uv2s);

            _hexMesh.SetTriangles(_triangles, 0);
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
            var vertexIndex = _vertices.Count;
            //consider perturbing
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        /// Add a triangle verbatim, without perturbing the positions.
        /// </summary>
        /// <param name="v1">First vertex position.</param>
        /// <param name="v2">Second vertex position.</param>
        /// <param name="v3">Third vertex position.</param>
        public void AddTriangleUnperturbed(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
        }

        /// <summary>
        ///     Add UV coordinates for a triangle.
        /// </summary>
        /// <param name="uv1">First UV coordinates.</param>
        /// <param name="uv2">Second UV coordinates.</param>
        /// <param name="uv3">Third UV coordinates.</param>
        public void AddTriangleUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            _uvs.Add(uv1);
            _uvs.Add(uv2);
            _uvs.Add(uv3);
        }

        /// <summary>
        ///     Add UV2 coordinates for a triangle.
        /// </summary>
        /// <param name="uv1">First UV2 coordinates.</param>
        /// <param name="uv2">Second UV2 coordinates.</param>
        /// <param name="uv3">Third UV2 coordinates.</param>
        public void AddTriangleUV2(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            _uv2s.Add(uv1);
            _uv2s.Add(uv2);
            _uv2s.Add(uv3);
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
            _cellIndices.Add(indices);
            _cellIndices.Add(indices);
            _cellIndices.Add(indices);
            _cellWeights.Add(weights1);
            _cellWeights.Add(weights2);
            _cellWeights.Add(weights3);
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
            var vertexIndex = _vertices.Count;
            //consider perturbing
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
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
            var vertexIndex = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _vertices.Add(v4);
            _triangles.Add(vertexIndex);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 1);
            _triangles.Add(vertexIndex + 2);
            _triangles.Add(vertexIndex + 3);
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
            _uvs.Add(uv1);
            _uvs.Add(uv2);
            _uvs.Add(uv3);
            _uvs.Add(uv4);
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
            _uv2s.Add(uv1);
            _uv2s.Add(uv2);
            _uv2s.Add(uv3);
            _uv2s.Add(uv4);
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
            _uvs.Add(new Vector2(uMin, vMin));
            _uvs.Add(new Vector2(uMax, vMin));
            _uvs.Add(new Vector2(uMin, vMax));
            _uvs.Add(new Vector2(uMax, vMax));
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
            _uv2s.Add(new Vector2(uMin, vMin));
            _uv2s.Add(new Vector2(uMax, vMin));
            _uv2s.Add(new Vector2(uMin, vMax));
            _uv2s.Add(new Vector2(uMax, vMax));
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
            _cellIndices.Add(indices);
            _cellIndices.Add(indices);
            _cellIndices.Add(indices);
            _cellIndices.Add(indices);
            _cellWeights.Add(weights1);
            _cellWeights.Add(weights2);
            _cellWeights.Add(weights3);
            _cellWeights.Add(weights4);
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