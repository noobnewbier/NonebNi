using System.Collections.Generic;
using System.Linq;
using Code.NonebNi.EditorComponent.Entities;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;

namespace NonebNi.Editors.Level.Entities
{
    public class EditorEntityPositioningService
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private readonly IReadOnlyMap _map;

        public EditorEntityPositioningService(CoordinateAndPositionService coordinateAndPositionService, IReadOnlyMap map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
        }

        /// <summary>
        /// Find all overlapping coordinates of any given <see cref="Entity" />, note it also returns coordinates out of bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(Entity entity)
        {
            if (!entity.IsCorrectSetUp) yield break;

            //bounding collider is defined only when entity is initialized
            var boundingCollider = entity.BoundingCollider;

            var searchedCoordinate = new HashSet<Coordinate>();
            var toSearchCoordinate = new Stack<Coordinate>();
            toSearchCoordinate.Push(_coordinateAndPositionService.NearestCoordinateForPoint(boundingCollider.bounds.center));

            while (toSearchCoordinate.Any())
            {
                var coordinate = toSearchCoordinate.Pop();
                if (searchedCoordinate.Contains(coordinate)) continue;
                if (!_map.IsCoordinateWithinMap(coordinate)) continue;

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