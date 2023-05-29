using System.Linq;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities;
using NonebNi.EditorComponent.Entities.Tile;
using NonebNi.EditorComponent.Entities.Unit;
using NonebNi.LevelEditor.Level.Entities;
using UnityEngine.SceneManagement;

namespace NonebNi.LevelEditor.Level.Maps
{
    /// <summary>
    ///     Synchronizing the <see cref="EditorMap" /> with the given <see cref="Scene" />
    /// </summary>
    public class MapSyncService
    {
        private readonly EditorEntityPositioningService _editorEntityPositioningService;
        private readonly IEditorMap _editorMap;
        private Scene _scene;

        public MapSyncService(EditorEntityPositioningService editorEntityPositioningService,
            Scene scene,
            IEditorMap editorMap)
        {
            _editorEntityPositioningService = editorEntityPositioningService;
            _scene = scene;
            _editorMap = editorMap;
        }

        public void Sync()
        {
            //reset the editorMap to initial state
            var allCoordinates = _editorMap.GetAllCoordinates();
            foreach (var coordinate in allCoordinates)
            {
                _editorMap.Set(coordinate, TileData.Default);
                _editorMap.Set<EditorEntityData<UnitData>>(coordinate, null);
            }

            var allEntities = _scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<EditorEntity>());
            foreach (var entity in allEntities.Where(e => e.IsCorrectSetUp))
            {
                entity.RefreshCache();

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
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        public bool SyncUnit(Unit unit)
        {
            if (!unit.IsCorrectSetUp) return false;

            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(unit).ToArray();
            if (coordinates.Length != 1) return false;

            var targetCoord = coordinates.First();
            if (_editorMap.Has<EditorEntityData<UnitData>>(targetCoord)) return false;

            //removing it from the existing coord
            _editorMap.Remove(unit.EntityData!);

            _editorMap.Set(targetCoord, unit.EntityData);
            return true;
        }

        public bool SyncTileModifier(TileModifier tileModifier)
        {
            if (!tileModifier.IsCorrectSetUp) return false;

            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(tileModifier).ToArray();
            if (!coordinates.Any()) return false;

            //removing it from the existing coord
            _editorMap.Remove(tileModifier.EntityData!);

            foreach (var coordinate in coordinates) _editorMap.Set(coordinate, tileModifier.EntityData);

            return true;
        }
    }
}