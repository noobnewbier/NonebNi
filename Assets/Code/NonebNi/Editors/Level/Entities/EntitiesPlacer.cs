﻿using System;
using System.Linq;
using Code.NonebNi.EditorComponent.Entities;
using Code.NonebNi.EditorComponent.Entities.Tile;
using Code.NonebNi.EditorComponent.Entities.Unit;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Editors.Level.Error;
using NonebNi.Editors.Level.Maps;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editors.Level.Entities
{
    /// <summary>
    /// Monitor the placement of <see cref="Entity" /> within the active scene, and change the <see cref="Map" /> accordingly
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
            var entities = selections.SelectMany(s => s.GetComponentsInChildren<Entity>()).ToArray();

            var invalidEntities = _errorChecker.CheckForErrors(entities).Select(e => e.ErrorSource).Distinct();
            var validEntities = entities.Except(invalidEntities);
            //we keep the invalid entities where they were, and only update the valid entities.
            //9 out of 10 this will be the intended behaviour(the invalid entities might only be temporarily invalid as the user is fiddling with size of colliders etc)
            foreach (var entity in validEntities) UpdateEntity(entity);
        }

        private void UpdateEntity(Entity entity)
        {
            if (entity == null) return;

            var coordinates = _editorEntityPositioningService.FindOverlappedCoordinates(entity).ToArray();

            switch (entity)
            {
                case Unit unit:
                    UpdateUnit(unit, coordinates);
                    break;
                case TileModifier tileModifier:
                    UpdateTiles(tileModifier, coordinates);
                    break;
            }

            Debug.Log($"{entity.name}");
            foreach (var coordinate in coordinates) Debug.Log(coordinate);
        }

        private void UpdateTiles(TileModifier tileModifier, Coordinate[] coordinates)
        {
            throw new NotImplementedException();
        }

        private void UpdateUnit(Unit unit, Coordinate[] coordinates)
        {
            var coordinateCount = coordinates.Length;
            Debug.Assert(
                coordinateCount == 1,
                $"The method expects the unit {unit.name} is in a valid state, but it was not"
            );
        }
    }
}