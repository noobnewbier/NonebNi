using NonebNi.Core.Entities;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Code.NonebNi.EditorComponent.Entities
{
    public partial class Entity<T> where T : EntityData
    {
        [CustomEditor(typeof(Entity<>))]
        [CanEditMultipleObjects]
        public class EntityEditor : Editor
        {
            private SerializedProperty _boundingColliderProp = null!;
            private SerializedProperty _entityDataSourceProp = null!;

            private void OnEnable()
            {
                _boundingColliderProp = serializedObject.FindProperty(nameof(boundingCollider));
                _entityDataSourceProp = serializedObject.FindProperty(nameof(entityDataSource));
            }

            public override void OnInspectorGUI()
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.ObjectField(
                        "Script",
                        MonoScript.FromMonoBehaviour((MonoBehaviour)target),
                        GetType(),
                        false
                    );
                }

                EditorGUILayout.PropertyField(_boundingColliderProp);
                EditorGUILayout.PropertyField(_entityDataSourceProp);

                serializedObject.ApplyModifiedProperties();

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (targets.Length > 1)
                    {
                        GUILayout.Label("Multi editing - error checking is not available", NonebGUIStyle.Hint);
                    }
                    else
                    {
                        var entity = (Entity<T>)target;

                        if (entity.IsCorrectSetUp)
                        {
                            GUILayout.Label("All Check Clear", NonebGUIStyle.Normal);
                        }
                        else
                        {
                            if (entity.entityDataSource == null) GUILayout.Label("Missing Data Source", NonebGUIStyle.Error);

                            if (entity.boundingCollider == null)
                                GUILayout.Label("Missing Bounding Collider", NonebGUIStyle.Error);
                        }
                    }
                }
            }
        }
    }
}