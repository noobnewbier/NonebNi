using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level.Maps
{
    /// <summary>
    /// Generate a <see cref="Map" /> given a <see cref="Scene" />, <see cref="MapConfigScriptable" /> and
    /// <seealso cref="WorldConfigScriptable" />
    /// Todo: we need to create a variant where the scene settings is token into account
    /// </summary>
    public class MapEditingService
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private readonly Map _map;

        public MapEditingService(CoordinateAndPositionService coordinateAndPositionService, Map map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
        }

        public void RefreshMapWithScene(Scene scene)
        {
            //reset the map to initial state
            var allCoordinates = _map.GetAllCoordinates();
        }

        /// <summary>
        /// Find all overlapping coordinates of any given <see cref="Entity" />, note it also returns coordinates out of bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(Entity entity)
        {
            if (!entity.IsCorrectSetUp) yield break;

            //bounding collider is defined only when entity is initialized
            var boundingCollider = entity.BoundingCollider!;

            var searchedCoordinate = new HashSet<Coordinate>();
            var toSearchCoordinate = new Stack<Coordinate>();
            toSearchCoordinate.Push(_coordinateAndPositionService.NearestCoordinateForPoint(boundingCollider.bounds.center));

            while (toSearchCoordinate.Any())
            {
                var coordinate = toSearchCoordinate.Pop();
                if (searchedCoordinate.Contains(coordinate)) continue;

                searchedCoordinate.Add(coordinate);

                var coordinatePos = _coordinateAndPositionService.FindPosition(coordinate);
                var closestPosOnBoundToCoordinate = boundingCollider.ClosestPoint(coordinatePos);

                var overLapped = _coordinateAndPositionService.IsPointWithinCoordinate(
                    closestPosOnBoundToCoordinate,
                    coordinate
                );
                if (overLapped)
                {
                    foreach (var neighbour in coordinate.Neighbours) toSearchCoordinate.Push(neighbour);

                    yield return coordinate;
                }
            }
        }
    }
}