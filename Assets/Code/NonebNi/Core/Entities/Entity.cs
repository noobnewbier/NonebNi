﻿using UnityEngine;

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
        [SerializeField] protected Collider? boundingCollider;

        /// <summary>
        /// We expect this is only called when the entity is <see cref="IsCorrectSetUp" />,
        /// in which case the underlying bounding collider is not null
        /// </summary>
        public Collider BoundingCollider => boundingCollider!;

        public virtual bool IsCorrectSetUp => boundingCollider != null;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.125f);
        }
    }

    public abstract partial class Entity<T> : Entity where T : EntityData
    {
        [SerializeField] protected EntityDataSource<T>? entityDataSource;

        private T? _cacheEntityData;

        public T? EntityData
        {
            get
            {
                //the backing data source can be removed in editor
                if (entityDataSource == null)
                {
                    _cacheEntityData = null;
                }
                else
                {
                    if (_cacheEntityData == null && entityDataSource != null)
                        _cacheEntityData = entityDataSource.CreateData();
                }

                return _cacheEntityData;
            }
        }

        public override bool IsCorrectSetUp => base.IsCorrectSetUp && EntityData != null;
    }
}