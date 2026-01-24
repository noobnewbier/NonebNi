using System;
using UnityEngine;

namespace NonebNi.EditorComponent.Entities
{
    public abstract class EntityDataSource : ScriptableObject { }

    public abstract class EditorEntityDataSource<T> : EntityDataSource where T : EditorEntityData
    {
        [SerializeField] protected Sprite icon = null!;
        [SerializeField] protected string entityName = null!;

        public abstract T CreateData(Guid guid, string factionId);
    }
}