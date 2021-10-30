using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level.Error
{
    public class ErrorChecker
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private Scene _scene;

        public ErrorChecker(CoordinateAndPositionService coordinateAndPositionService, Scene scene)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _scene = scene;
        }

        public IEnumerable<ErrorEntry> CheckForErrors(IEnumerable<Entity> changedEntities)
        {
            var changedEntitiesAsArray = changedEntities.ToArray();
            var units = changedEntitiesAsArray.OfType<Unit>().ToArray();
            var tileModifiers = changedEntitiesAsArray.OfType<TileModifier>().ToArray();

            var multiCoordUnits = CheckForMultiCoordUnit(units);
            var overlappingEntities = CheckForOverlappingEntities(units, tileModifiers);
            var noCoordEntities = CheckForNoCoordinateEntities(changedEntitiesAsArray);

            return multiCoordUnits.Concat(overlappingEntities).Concat(noCoordEntities);
        }

        public IEnumerable<ErrorEntry> CheckForErrors()
        {
            var allEntities = _scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Entity>()).ToArray();

            return CheckForErrors(allEntities);
        }

        private IEnumerable<ErrorEntry> CheckForOverlappingEntities(IEnumerable<Unit> units,
                                                                    IEnumerable<TileModifier> tileModifiers)
        {
            var allEntities = _scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Entity>()).ToArray();

            //todo: in theory we could have cached the coordinates to avoid unnecessary calculation, but we don't really need this for now
            IEnumerable<ErrorEntry> FindOverlappingEntitiesOfType<T>(IEnumerable<T> enumerable) where T : Entity
            {
                var allEntityOfTypeAndCoordinates = allEntities.OfType<T>()
                                                               .Select(
                                                                   u => (u,
                                                                       _coordinateAndPositionService
                                                                           .FindOverlappedCoordinates(u)
                                                                           .ToArray())
                                                               )
                                                               .ToArray();
                var entityToCheckAndCoordinates = enumerable
                                                  .Select(
                                                      u => (u,
                                                          _coordinateAndPositionService.FindOverlappedCoordinates(u)
                                                              .ToArray())
                                                  )
                                                  .ToArray();
                foreach (var (entity, coordinates) in entityToCheckAndCoordinates)
                foreach (var (e, cs) in allEntityOfTypeAndCoordinates)
                {
                    if (e == entity) continue;
                    if (cs.Intersect(coordinates).Any())
                        yield return new ErrorEntry(entity, $"{entity.name} is overlapping with {e.name}");
                }
            }

            foreach (var errorEntry in FindOverlappingEntitiesOfType(units)) yield return errorEntry;
            foreach (var errorEntry in FindOverlappingEntitiesOfType(tileModifiers)) yield return errorEntry;
        }

        private IEnumerable<ErrorEntry> CheckForNoCoordinateEntities(IEnumerable<Entity> entities) =>
            from entity in entities
            let coordinates = _coordinateAndPositionService.FindOverlappedCoordinates(entity)
            where !coordinates.Any()
            select new ErrorEntry(entity, $"{entity.name} is not placed on a valid coordinate");

        private IEnumerable<ErrorEntry> CheckForMultiCoordUnit(IEnumerable<Unit> units)
        {
            return from unit in units
                let coordinates = _coordinateAndPositionService.FindOverlappedCoordinates(unit).ToArray()
                where coordinates.Length > 1
                select new ErrorEntry(
                    unit,
                    $"{unit.name} is overlapping with multiple Coords {string.Join(",", coordinates.SelectMany(c => c.ToString()))}"
                );
        }
    }
}