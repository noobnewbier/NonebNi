using UnityEngine;

namespace NonebNi.Core.Entities
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