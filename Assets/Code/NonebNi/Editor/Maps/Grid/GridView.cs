using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps.Grid
{
    public class GridView
    {
        private bool _isDrawing;
        private Map? _map;
        private GridPresenter _presenter;
        private WorldConfig? _worldConfig;

        public GridView()
        {
            _presenter = new GridPresenter(this);
        }

        public void StartDrawGridWithData(Map map, WorldConfig worldConfig)
        {
            _isDrawing = true;
            _worldConfig = worldConfig;
            _map = map;
        }

        public void StopDrawing()
        {
            _isDrawing = false;
        }

        public void DrawGrid()
        {
            if (!_isDrawing) return;
            if (_map == null) return;
            if (_worldConfig == null) return;

            var grid = _map.GetGridForType<Tile>();

            var positions = from Tile? tile in grid
                where tile != null
                select GetTilePosition(_worldConfig.MapStartingPosition.y, new Coordinate(tile.Coordinate.X, tile.Coordinate.Z), _worldConfig);

            var vertices = (from pos in positions from corner in _worldConfig.TileCornersOffset select pos + corner).ToList();
            for (var i = 1; i < vertices.Count; i++) Handles.DrawLine(vertices[i], i % 6 == 5 ? vertices[i - 5] : vertices[i + 1]);
        }

        private Vector3 GetTilePosition(float yPosition, Coordinate coordinate, WorldConfig worldConfig)
        {
            var upDistance = worldConfig.OuterRadius * 1.5f;
            var sideDistance = worldConfig.InnerRadius * 2f;
            var sideOffset = coordinate.Z % 2 * sideDistance / 2f;

            return new Vector3(coordinate.X * sideDistance + sideOffset, yPosition, coordinate.Z * upDistance) + worldConfig.MapStartingPosition;
        }
    }
}