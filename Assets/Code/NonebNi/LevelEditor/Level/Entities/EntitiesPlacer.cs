﻿using System.Linq;
using NonebNi.EditorComponent.Entities;
using NonebNi.EditorComponent.Entities.Tile;
using NonebNi.EditorComponent.Entities.Unit;
using NonebNi.LevelEditor.Level.Error;
using NonebNi.LevelEditor.Level.Maps;
using UnityEditor;

namespace NonebNi.LevelEditor.Level.Entities
{
    /// <summary>
    ///     Monitor the placement of <see cref="EditorEntity" /> within the active scene, and change the <see cref="EditorMap" />
    ///     accordingly
    ///     todo: register to undo callback
    ///     todo: deal with overlapping(error log etc)
    ///     todo: deal with deletion
    /// </summary>
    public class EntitiesPlacer
    {
        private readonly EditorEntitySyncService _editorEntitySyncService;
        private readonly ErrorChecker _errorChecker;

        public EntitiesPlacer(
            EditorEntitySyncService entityService,
            ErrorChecker errorChecker)
        {
            _editorEntitySyncService = entityService;
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

            switch (editorEntity)
            {
                case Unit unit:
                    UpdateUnit(unit);
                    break;
                case TileModifier tileModifier:
                    UpdateTiles(tileModifier);
                    break;
            }
        }

        private void UpdateTiles(TileModifier tileModifier)
        {
            _editorEntitySyncService.SyncTileModifier(tileModifier);
        }

        private void UpdateUnit(Unit unit)
        {
            _editorEntitySyncService.SyncUnit(unit);
        }
    }
}