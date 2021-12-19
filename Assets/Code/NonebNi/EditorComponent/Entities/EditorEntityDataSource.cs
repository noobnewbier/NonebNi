using System;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    public abstract class EntityDataSource : ScriptableObject
    {
    }

    public abstract class EditorEntityDataSource<T> : EntityDataSource where T : EditorEntityData
    {
        [SerializeField] protected Sprite icon = null!;
        [SerializeField] protected string entityName = null!;

        public abstract T CreateData(Guid guid);
    }
}