using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.EditorComponent.Entities;
using NonebNi.Editors.Level.Data;

namespace NonebNi.Editors.Level.Entities
{
    public class EditorEntityPositioningService
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private readonly IEditorMap _map;

        public EditorEntityPositioningService(CoordinateAndPositionService coordinateAndPositionService, IEditorMap map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
        }

        /// <summary>
        /// Find all overlapping coordinates of any given <see cref="EditorEntity" />, note it also returns coordinates out of
        /// bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(EditorEntity editorEntity)
        {
            if (!editorEntity.IsCorrectSetUp) yield break;

            //bounding collider is defined only when editorEntity is initialized
            var boundingCollider = editorEntity.BoundingCollider;

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