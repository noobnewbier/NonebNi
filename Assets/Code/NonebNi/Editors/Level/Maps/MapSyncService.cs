﻿using System;
using System.Linq;
using Code.NonebNi.EditorComponent.Entities;
using Code.NonebNi.EditorComponent.Entities.Tile;
using Code.NonebNi.EditorComponent.Entities.Unit;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Level.Entities;
using NonebNi.Game.Tiles;
using NonebNi.Game.Units;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Level.Maps
{
    /// <summary>
    /// Synchronizing the <see cref="EditorMap" /> with the given <see cref="Scene" />
    /// </summary>
    public class MapSyncService
    {
        private readonly EditorEntityPositioningService _editorEntityPositioningService;
        private readonly EditorMap _editorMap;
        private Scene _scene;

        public MapSyncService(EditorEntityPositioningService editorEntityPositioningService,
                              Scene scene,
                              EditorMap editorMap)
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
            if (!unit.IsCorrectSetUp) return false;

            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(unit).ToArray();
            if (coordinates.Length != 1) return false;

            var targetCoord = coordinates.First();
            if (_editorMap.Has<EditorEntityData<UnitData>>(targetCoord)) return false;

            //removing it from the existing coord
            if (_editorMap.TryFind(unit.EntityData!, out var currentCoord))
                _editorMap.Set<EditorEntityData<UnitData>>(currentCoord, null);

            _editorMap.Set(targetCoord, unit.EntityData);
            return true;
        }

        //todo: there should be an error code-ish thing so the caller can print debug info etc.
        [Obsolete]
        public bool SyncTileModifier(TileModifier tileModifier)
        {
            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(tileModifier).ToArray();
            if (!coordinates.Any()) return false;

            //todo: this doesn't deal with:
            // 1. when A, B modifier or coordinate C.
            // 2. remove A.
            // 3. C is not updated with B
            foreach (var coordinate in coordinates) _editorMap.Set(coordinate, tileModifier.EntityData);

            return true;
        }
    }
}