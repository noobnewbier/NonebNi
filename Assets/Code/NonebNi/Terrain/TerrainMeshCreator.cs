using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using UnityEngine;

namespace NonebNi.Terrain
{
    //TODO: incorporate this to editor :) fml
    //TODO: I wrote this thing, and I have no idea wtf does this do.
    public interface ITerrainMeshCreator
    {
        /// <summary>
        ///     Triangulate everything in the chunk.
        /// </summary>
        Mesh Triangulate();
    }

    public class TerrainMeshCreator : ITerrainMeshCreator
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly List<Coordinate> _coordinates;
        private readonly IReadOnlyMap _map;

        private readonly TerrainMeshData _terrain;
        private readonly Color _weights1 = Color.red;
        private readonly Color _weights2 = Color.green;
        private readonly Color _weights3 = Color.blue;

        public TerrainMeshCreator(
            TerrainMeshData terrain,
            ICoordinateAndPositionService coordinateAndPositionService,
            IReadOnlyMap map)
        {
            _terrain = terrain;
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;

            _coordinates = _map.GetAllCoordinates().ToList();
        }

        /// <summary>
        ///     Triangulate everything in the chunk.
        /// </summary>
        public Mesh Triangulate()
        {
            foreach (var t in _coordinates)
                Triangulate(t);

            return _terrain.Apply();
        }

        private void Triangulate(Coordinate cell)
        {
            foreach (var direction in HexDirection.All) Triangulate(direction, cell);
        }

        private void Triangulate(HexDirection direction, Coordinate cell)
        {
            var coordinatePos = _coordinateAndPositionService.FindPosition(cell);
            var e = new EdgeVertices(
                _coordinateAndPositionService.GetFirstSolidCorner(cell, direction),
                _coordinateAndPositionService.GetSecondSolidCorner(cell, direction)
            );

            TriangulateEdgeFan(coordinatePos, e, _coordinates.IndexOf(cell));

            if (direction.LessThanOrEqualWith(HexDirection.SouthEast)) TriangulateConnection(direction, cell, e);
        }

        private void TriangulateConnection(
            HexDirection direction,
            Coordinate coord,
            EdgeVertices e1)
        {
            var neighbor = coord + direction;
            if (!_map.IsCoordinateWithinMap(neighbor)) return;

            var bridge = _coordinateAndPositionService.GetBridge(direction);

            var e2 = new EdgeVertices(
                e1.V1 + bridge,
                e1.V5 + bridge
            );

            TriangulateEdgeStrip(
                e1,
                _weights1,
                _coordinates.IndexOf(coord),
                e2,
                _weights2,
                _coordinates.IndexOf(neighbor)
            );
            var nextNeighbor = coord + direction.NextDirection();
            if (
                direction.LessThanOrEqualWith(HexDirection.East) &&
                _map.IsCoordinateWithinMap(nextNeighbor)
            )
            {
                var v5 = e1.V5 + _coordinateAndPositionService.GetBridge(direction.NextDirection());

                TriangulateCorner(
                    e1.V5,
                    coord,
                    e2.V5,
                    neighbor,
                    v5,
                    nextNeighbor
                );
            }
        }

        private void TriangulateCorner(
            Vector3 bottom,
            Coordinate bottomCell,
            Vector3 left,
            Coordinate leftCell,
            Vector3 right,
            Coordinate rightCell
        )
        {
            _terrain.AddTriangle(bottom, left, right);
            var indices = new Vector3(
                _coordinates.IndexOf(bottomCell),
                _coordinates.IndexOf(leftCell),
                _coordinates.IndexOf(rightCell)
            );
            _terrain.AddTriangleCellData(indices, _weights1, _weights2, _weights3);
        }

        private void TriangulateEdgeFan(Vector3 center, EdgeVertices edge, int index)
        {
            _terrain.AddTriangle(center, edge.V1, edge.V2);
            _terrain.AddTriangle(center, edge.V2, edge.V3);
            _terrain.AddTriangle(center, edge.V3, edge.V4);
            _terrain.AddTriangle(center, edge.V4, edge.V5);

            var indices = Vector3.one * index;
            var weights1 = Color.red; //TODO: wt is weight?
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
            _terrain.AddTriangleCellData(indices, weights1);
        }

        private void TriangulateEdgeStrip(
            EdgeVertices e1,
            Color w1,
            float index1,
            EdgeVertices e2,
            Color w2,
            float index2
        )
        {
            _terrain.AddQuad(e1.V1, e1.V2, e2.V1, e2.V2);
            _terrain.AddQuad(e1.V2, e1.V3, e2.V2, e2.V3);
            _terrain.AddQuad(e1.V3, e1.V4, e2.V3, e2.V4);
            _terrain.AddQuad(e1.V4, e1.V5, e2.V4, e2.V5);

            Vector3 indices;
            indices.x = indices.z = index1;
            indices.y = index2;
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
            _terrain.AddQuadCellData(indices, w1, w2);
        }
    }
}