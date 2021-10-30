using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEngine.SceneManagement;

namespace NonebNi.Editor.Level.Maps
{
    /// <summary>
    /// Synchronizing the <see cref="Map" /> with the given <see cref="Scene" />
    /// </summary>
    public class MapSyncService
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private readonly Map _map;
        private Scene _scene;

        public MapSyncService(CoordinateAndPositionService coordinateAndPositionService, Scene scene, Map map)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _scene = scene;
            _map = map;
        }

        public void Sync()
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
                        SyncUnit(unit);
                        break;
                    case TileModifier tileModifier:
                        SyncTileModifier(tileModifier);
                        break;
                }
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        public bool SyncUnit(Unit unit)
        {
            var coordinates = _coordinateAndPositionService.FindOverlappedCoordinates(unit).ToArray();
            if (coordinates.Length != 1) return false;

            var coord = coordinates.First();
            if (_map.Has<UnitData>(coord)) return false;

            _map.Set(coord, unit.EntityData);
            return true;
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        public bool SyncTileModifier(TileModifier tileModifier)
        {
            var coordinates = _coordinateAndPositionService.FindOverlappedCoordinates(tileModifier).ToArray();
            if (!coordinates.Any()) return false;

            foreach (var coordinate in coordinates) _map.Set(coordinate, tileModifier.EntityData);

            return true;
        }
    }
}