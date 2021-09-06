using System;
using UnityEngine;

namespace NonebNi.Core.Entities
{
    /// <summary>
    /// An <see cref="Entity" /> is something that can be placed on the board within SceneView.
    /// This is necessary wrapper of <see cref="EntityData" /> to avoid the internal data being filled with
    /// <see cref="MonoBehaviour" />.
    /// An entity must at least be within one tile, it can potentially spans through multiple tile.
    /// Which tiles an <see cref="Entity" /> is at/spanning through depends on the underlying bounding box.
    /// We find out what an entity is by the other component it is holding(and create the level of the game by finding out all
    /// entities if the map).
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        [SerializeField] private Collider? boundingCollider;

        public Collider? BoundingCollider
        {
            get
            {
                if (boundingCollider == null)
                    throw new InvalidOperationException(
                        $"This entity {name} is not constructed properly - there are no {boundingCollider}"
                    );

                return boundingCollider;
            }
        }

        public abstract bool IsInitialized { get; }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.125f);
        }
    }

    public abstract class Entity<T> : Entity where T : EntityData
    {
        [SerializeField] protected EntityDataSource<T>? entityDataSource;

        private T? _cacheEntityData;

        public T? EntityData
        {
            get
            {
                if (_cacheEntityData == null && entityDataSource != null) _cacheEntityData = entityDataSource.CreateData();

                return _cacheEntityData;
            }
        }

        public override bool IsInitialized => BoundingCollider != null && EntityData != null;
    }
}