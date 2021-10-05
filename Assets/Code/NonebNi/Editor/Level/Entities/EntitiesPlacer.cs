using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Units;
using NonebNi.Editor.Level.Maps;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Entities
{
    /// <summary>
    /// Monitor the placement of <see cref="Entity" /> within the active scene, and change the <see cref="Map" /> accordingly
    /// todo: register to undo callback
    /// </summary>
    public class EntitiesPlacer
    {
        private readonly MapEditingService _mapEditingService;

        public EntitiesPlacer(MapEditingService entityService)
        {
            _mapEditingService = entityService;
        }

        public void UpdateEntitiesPlacement()
        {
            var selections = Selection.transforms;
            var entities = selections.SelectMany(s => s.GetComponentsInChildren<Entity>());

            foreach (var entity in entities) UpdateEntity(entity);
        }

        private void UpdateEntity(Entity entity)
        {
            if (entity == null) return;

            var coordinates = _mapEditingService.FindOverlappedCoordinates(entity).ToArray();

            //todo: if no overlap

            //todo: place stuffs on map, change weight etc
            switch (entity)
            {
                case Unit unit:
                    UpdateUnit(unit, coordinates);

                    break;
            }

            Debug.Log($"{entity.name}");
            foreach (var coordinate in coordinates) Debug.Log(coordinate);
        }

        private void UpdateUnit(Unit unit, Coordinate[] coordinates)
        {
            var coordinateCount = coordinates.Length;
            switch (coordinateCount)
            {
                case 1:
                    break;

                case 0:
                    //todo: come up with an mechanism to deal with this.
                    break;
            }
        }

        private void PlaceUnitInMap(UnitData unitData, Coordinate coordinate)
        {
        }
    }
}