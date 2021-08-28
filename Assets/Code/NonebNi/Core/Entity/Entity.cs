using System;
using UnityEngine;

namespace NonebNi.Core.Entity
{
    /// <summary>
    /// An <see cref="Entity" /> is something that can be placed on the board.
    /// An entity must at least be within one tile, it can potentially spans through multiple tile.
    /// Which tiles an <see cref="Entity" /> is at/spanning through depends on the underlying bounding box.
    /// We find out what an entity is by the other component it is holding(and create the level of the game by finding out all
    /// entities if the map).
    /// </summary>
    public class Entity : MonoBehaviour
    {
        [SerializeField] private Collider? boundingCollider;

        public Collider BoundingCollider
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.125f);
        }
    }
}