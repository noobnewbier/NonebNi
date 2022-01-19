using System;
using System.Linq;
using NonebNi.Core.Entities;
using NonebNi.Ui.Entities;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityUtils.SerializableGuid;

namespace NonebNi.EditorComponent.Entities
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
        [SerializeField] protected SerializableGuid serializableGuid;
        [SerializeField] private Entity? entity;

        /// <summary>
        /// We expect this is only called when the editorEntity is <see cref="IsCorrectSetUp" />,
        /// in which case the underlying bounding collider is not null
        /// </summary>
        public Collider BoundingCollider => boundingCollider!;

        public virtual bool IsCorrectSetUp => boundingCollider != null;

        protected Guid Guid => serializableGuid;

        private void Update()
        {
            OnValidate();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawSphere(transform.position, 0.125f);
        }

        private void OnValidate()
        {
            ValidateGuid();
            ValidateEntity();

            //Check the prefab section: https://blog.unity.com/technology/spotlight-team-best-practices-guid-based-references
            var prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(this);
            if (prefabInstanceStatus == PrefabInstanceStatus.Connected)
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);

            void ValidateGuid()
            {
                var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                var isPrefabAsset = PrefabUtility.IsPartOfPrefabAsset(this);
                var isEditingPrefab = currentPrefabStage != null && currentPrefabStage == StageUtility.GetStage(gameObject);
                if (isEditingPrefab || isPrefabAsset)
                {
                    serializableGuid.Value = Guid.Empty;
                    EditorUtility.SetDirty(gameObject);
                }
                else
                {
                    var otherEditorEntity = FindObjectsOfType<EditorEntity>().Where(e => e != this);
                    if (Guid == Guid.Empty || otherEditorEntity.Any(e => e.Guid == Guid))
                    {
                        serializableGuid.Value = Guid.NewGuid();
                        EditorUtility.SetDirty(gameObject);
                    }
                }
            }

            void ValidateEntity()
            {
                if (entity == null)
                {
                    entity = GetComponent<Entity>();
                    if (entity == null)
                    {
                        entity = gameObject.AddComponent<Entity>();

                        // ReSharper disable once Unity.InefficientPropertyAccess
                        EditorUtility.SetDirty(gameObject);
                    }
                }

                if (entity.guid != Guid)
                {
                    entity.guid.Value = Guid;
                    EditorUtility.SetDirty(gameObject);
                }
            }
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