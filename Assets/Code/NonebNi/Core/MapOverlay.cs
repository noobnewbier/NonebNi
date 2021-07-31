using System.Collections.Generic;
using System.Linq;
using Noneb.Core.Game.WorldConfigurations;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using UnityEngine;

namespace Noneb.Ui.Game.Maps.Create
{
    public class MapOverlay : MonoBehaviour
    {
        [SerializeField] private MapConfig mapConfig;
        [SerializeField] private WorldConfig worldConfig;


        private void OnDrawGizmos()
        {
            var positions = new List<Vector3>();
            for (var i = 0; i < mapConfig.GetMap2DActualHeight(); i++)
            for (var j = 0; j < mapConfig.GetMap2DActualWidth(); j++)
            {
                positions.Add(GetTilePosition(worldConfig, transform.position.y, new Coordinate(j, i)));
            }

            var vertices = (from pos in positions from corner in worldConfig.TileCornersOffset select pos + corner).ToList();
            for (var i = 1; i < vertices.Count; i++) Gizmos.DrawLine(vertices[i], i % 6 == 5 ? vertices[i - 5] : vertices[i + 1]);
        }

        private static Vector3 GetTilePosition(WorldConfig worldConfig, float yPosition, Coordinate coordinate)
        {
            var upDistance = worldConfig.OuterRadius * 1.5f;
            var sideDistance = worldConfig.InnerRadius * 2f;
            var sideOffset = coordinate.Z % 2 * sideDistance / 2f;

            return new Vector3(coordinate.X * sideDistance + sideOffset, yPosition, coordinate.Z * upDistance);
        }
    }
}