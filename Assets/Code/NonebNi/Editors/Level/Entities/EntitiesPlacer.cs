using System.Linq;
using NonebNi.EditorComponent.Entities;
using NonebNi.EditorComponent.Entities.Tile;
using NonebNi.EditorComponent.Entities.Unit;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Maps;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editors.Level.Entities
{
    /// <summary>
    /// Monitor the placement of <see cref="EditorEntity" /> within the active scene, and change the <see cref="EditorMap" />
    /// accordingly
    /// todo: register to undo callback
    /// todo: deal with overlapping(error log etc)
    /// todo: deal with deletion
    /// </summary>
    public class EntitiesPlacer
    {
        private readonly EditorEntityPositioningService _editorEntityPositioningService;
        private readonly ErrorChecker _errorChecker;
        private readonly MapSyncService _mapSyncService;

        public EntitiesPlacer(MapSyncService entityService,
                              EditorEntityPositioningService editorEntityPositioningService,
                              ErrorChecker errorChecker)
        {
            _mapSyncService = entityService;
            _editorEntityPositioningService = editorEntityPositioningService;
            _errorChecker = errorChecker;
        }

        public void UpdateEntitiesPlacement()
        {
            var selections = Selection.transforms;
            var entities = selections.SelectMany(s => s.GetComponentsInChildren<EditorEntity>()).ToArray();

            var invalidEntities = _errorChecker.CheckForErrors(entities).Select(e => e.ErrorSource).Distinct();
            var validEntities = entities.Except(invalidEntities);
            //we keep the invalid entities where they were, and only update the valid entities.
            //9 out of 10 this will be the intended behaviour(the invalid entities might only be temporarily invalid as the user is fiddling with size of colliders etc)
            foreach (var entity in validEntities) UpdateEntity(entity);
        }

        private void UpdateEntity(EditorEntity editorEntity)
        {
            if (editorEntity == null) return;

            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(editorEntity).ToArray();

            switch (editorEntity)
            {
                case Unit unit:
                    UpdateUnit(unit);
                    break;
                case TileModifier tileModifier:
                    UpdateTiles(tileModifier);
                    break;
            }

            Debug.Log($"{editorEntity.name}");
            foreach (var coordinate in coordinates) Debug.Log(coordinate);
        }

        private void UpdateTiles(TileModifier tileModifier)
        {
            _mapSyncService.SyncTileModifier(tileModifier);
        }

        private void UpdateUnit(Unit unit)
        {
            _mapSyncService.SyncUnit(unit);
        }
    }
}