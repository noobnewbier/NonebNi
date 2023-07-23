using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.EditorComponent.Entities;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.Terrain;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Entities
{
    public interface IEditorEntityPositioningService
    {
        /// <summary>
        ///     Find all overlapping coordinates of any given <see cref="EditorEntity" />, note it also returns coordinates out of
        ///     bounds(tbd).
        /// </summary>
        IEnumerable<Coordinate> FindOverlappedCoordinates(EditorEntity editorEntity);

        IEnumerable<Coordinate> FindOverlappedCoordinates(Collider collider);
    }

    public class EditorEntityPositioningService : IEditorEntityPositioningService
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly IEditorMap _map;

        public EditorEntityPositioningService(ICoordinateAndPositionService coordinateAndPositionService, IEditorMap map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _map = map;
        }

        /// <summary>
        ///     Find all overlapping coordinates of any given <see cref="EditorEntity" />, note it also returns coordinates out of
        ///     bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(EditorEntity editorEntity) =>
            //bounding collider is defined only when editorEntity is initialized
            !editorEntity.IsCorrectSetUp ?
                Enumerable.Empty<Coordinate>() :
                FindOverlappedCoordinates(editorEntity.BoundingCollider);

        /// <summary>
        ///     Find all overlapping coordinates of any given <see cref="EditorEntity" />, note it also returns coordinates out of
        ///     bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(Collider collider)
        {
            var searchedCoordinate = new HashSet<Coordinate>();
            var toSearchCoordinate = new Stack<Coordinate>();
            toSearchCoordinate.Push(_coordinateAndPositionService.NearestCoordinateForPoint(collider.bounds.center));

            while (toSearchCoordinate.Any())
            {
                var coordinate = toSearchCoordinate.Pop();
                if (searchedCoordinate.Contains(coordinate)) continue;
                if (!_map.IsCoordinateWithinMap(coordinate)) continue;

                searchedCoordinate.Add(coordinate);

                var coordinatePos = _coordinateAndPositionService.FindPosition(coordinate);
                var closestPosOnBoundToCoordinate = collider.ClosestPoint(coordinatePos);

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