using System;
using System.Linq;
using NonebNi.Core.Entities;
using UnityEngine;

namespace Code.NonebNi.EditorComponent.Entities
{
    /// <summary>
    /// An <see cref="EditorEntity" /> is something that can be placed on the board within SceneView.
    /// This is necessary wrapper of <see cref="EntityData" /> to avoid the internal data being filled with
    /// <see cref="MonoBehaviour" />. This component is used to build the scene and will be stripped on build/play.
    /// An editorEntity must at least be within one tile, it can potentially spans through multiple tile.
    /// Which tiles an <see cref="EditorEntity" /> is at/spanning through depends on the underlying bounding box.
    /// We find out what an editorEntity is by the other component it is holding(and create the level of the game by finding out all
    /// entities if the map).
    /// </summary>
    [ExecuteInEditMode]
    public abstract class EditorEntity : MonoBehaviour
    {
        [SerializeField] protected Collider? boundingCollider;
        [SerializeField] protected byte[] serializedGuid = new byte[16]; //16 is the guid's length in bytes

        /// <summary>
        /// We expect this is only called when the editorEntity is <see cref="IsCorrectSetUp" />,
        /// in which case the underlying bounding collider is not null
        /// </summary>
        public Collider BoundingCollider => boundingCollider!;

        public virtual bool IsCorrectSetUp => boundingCollider != null;

        protected Guid Guid => new Guid(serializedGuid);

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.125f);
        }

        private void OnValidate()
        {
            var otherEditorEntity = FindObjectsOfType<EditorEntity>().Where(e => e != this);
            if (Guid == Guid.Empty || otherEditorEntity.Any(e => e.Guid == Guid))
                serializedGuid = Guid.NewGuid().ToByteArray();
        }
    }

    public abstract partial class EditorEntity<T> : EditorEntity where T : EditorEntityData
    {
        [SerializeField] protected EditorEntityDataSource<T>? entityDataSource;

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
                        _cacheEntityData = entityDataSource.CreateData(Guid);
                }

                return _cacheEntityData;
            }
        }

        public override bool IsCorrectSetUp => base.IsCorrectSetUp && EntityData != null;
    }
}