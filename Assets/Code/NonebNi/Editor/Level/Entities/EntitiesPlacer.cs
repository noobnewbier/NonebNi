using System.Linq;
using NonebNi.Core.Entity;
using NonebNi.Core.Maps;
using NonebNi.Editor.Di;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Entities
{
    /// <summary>
    /// Monitor the placement of <see cref="Entity" /> within the active scene, and change the <see cref="Map" /> accordingly
    /// </summary>
    public class EntitiesPlacer
    {
        private readonly EntityService _entityService;

        public EntitiesPlacer(ILevelEditorComponent component)
        {
            _entityService = component.EntityService;
        }

        public void UpdateEntitiesPlacement()
        {
            var selections = Selection.transforms;
            var entities = selections.SelectMany(s => s.GetComponentsInChildren<Entity>());

            foreach (var entity in entities) UpdateEntity(entity);
        }

        private void UpdateEntity(Entity entity)
        {
            var coordinates = _entityService.FindOverlappedCoordinates(entity);

            //todo: place stuffs on map, change weight etc
            Debug.Log($"{entity.name}");
            foreach (var coordinate in coordinates) Debug.Log(coordinate);
        }
    }
}