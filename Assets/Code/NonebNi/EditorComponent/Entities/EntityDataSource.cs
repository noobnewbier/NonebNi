using NonebNi.Core.Entities;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    public abstract class EntityDataSource : ScriptableObject
    {
    }


    public abstract class EntityDataSource<T> : EntityDataSource where T : EntityData
    {
        [SerializeField] protected Sprite icon = null!;
        [SerializeField] protected string entityName = null!;

        public abstract T CreateData();
    }
}