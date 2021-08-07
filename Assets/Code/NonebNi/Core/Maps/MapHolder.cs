using System;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Tiles;
using UnityEngine;

namespace NonebNi.Core.Maps
{
    /// <summary>
    /// A temporary class for holding a map such that others(editor tools etc) can interact with the map.
    /// Should be refactored/removed after we decide how to handle various dependencies
    /// </summary>
    public class MapHolder : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig = null!;
        [SerializeField] private WorldConfig worldConfig = null!;

        private readonly MapGenerationService _mapGenerationService = new MapGenerationService();
        private Lazy<Map> _mapLazy;

        private void Awake()
        {
            _mapLazy = new Lazy<Map>(
                () =>
                {
                    var map = _mapGenerationService.CreateMap(mapConfig, gameObject.scene);
                    return map;
                }
            );
        }

        private void OnDrawGizmos()
        {
            _mapLazy = new Lazy<Map>(
                () =>
                {
                    var map = _mapGenerationService.CreateMap(mapConfig, gameObject.scene);
                    return map;
                }
            );
            var map = _mapLazy.Value!;
            var grid = map.GetGridForType<Tile>();

            var positions = (from Tile? tile in grid
                where tile != null
                select GetTilePosition(transform.position.y, new Coordinate(tile.Coordinate.X, tile.Coordinate.Z))).ToList();

            var vertices = (from pos in positions from corner in worldConfig.TileCornersOffset select pos + corner).ToList();
            for (var i = 1; i < vertices.Count; i++) Gizmos.DrawLine(vertices[i], i % 6 == 5 ? vertices[i - 5] : vertices[i + 1]);
        }

        private Vector3 GetTilePosition(float yPosition, Coordinate coordinate)
        {
            var upDistance = worldConfig.OuterRadius * 1.5f;
            var sideDistance = worldConfig.InnerRadius * 2f;
            var sideOffset = coordinate.Z % 2 * sideDistance / 2f;

            return new Vector3(coordinate.X * sideDistance + sideOffset, yPosition, coordinate.Z * upDistance) + transform.position;
        }
    }
}