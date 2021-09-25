using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
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
        private Scene _scene;

        public MapEditingService(CoordinateAndPositionService coordinateAndPositionService, Scene scene, Map map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _scene = scene;
            _map = map;
        }

        public void RefreshMap()
        {
            //reset the map to initial state
            var allCoordinates = _map.GetAllCoordinates();
            foreach (var coordinate in allCoordinates)
            {
                _map.Set(coordinate, TileData.Default);
                _map.Set<UnitData>(coordinate, null);
            }

            var allEntities = _scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Entity>());
            foreach (var entity in allEntities.Where(e => e.IsCorrectSetUp))
                switch (entity)
                {
                    case Unit unit:
                        TryPlaceUnitInMap(unit);
                        break;
                    case TileModifier tileModifier:
                        TryPlaceTileOnMap(tileModifier);
                        break;
                }
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        public bool TryPlaceUnitInMap(Unit unit)
        {
            var coordinates = FindOverlappedCoordinates(unit).ToArray();
            if (coordinates.Length != 1) return false;

            var coord = coordinates.First();
            if (_map.Has<UnitData>(coord)) return false;

            _map.Set(coord, unit.EntityData);
            return true;
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        public bool TryPlaceTileOnMap(TileModifier tileModifier)
        {
            var coordinates = FindOverlappedCoordinates(tileModifier).ToArray();
            if (!coordinates.Any()) return false;

            foreach (var coordinate in coordinates) _map.Set(coordinate, tileModifier.EntityData);

            return true;
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