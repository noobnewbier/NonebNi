﻿using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;

namespace NonebNi.Core.Entity
{
    public class EntityService
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;

        public EntityService(CoordinateAndPositionService coordinateAndPositionService)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
        }

        /// <summary>
        /// Find all overlapping coordinates of any given <see cref="Entity" />, note it also returns coordinates out of bounds(tbd).
        /// </summary>
        public IEnumerable<Coordinate> FindOverlappedCoordinates(Entity entity)
        {
            var boundingCollider = entity.BoundingCollider;

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