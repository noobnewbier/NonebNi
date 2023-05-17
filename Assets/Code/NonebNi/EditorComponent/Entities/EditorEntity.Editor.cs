using System.Linq;
using NonebNi.Core.Factions;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.EditorComponent.Entities
{
    public partial class EditorEntity<T> where T : EditorEntityData
    {
        [CustomEditor(typeof(EditorEntity<>))]
        [CanEditMultipleObjects]
        public class EntityEditor : Editor
        {
            private SerializedProperty _boundingColliderProp = null!;
            private SerializedProperty _entityDataSourceProp = null!;
            private AutoCompleteField _factionIdField = null!;


            private void OnEnable()
            {
                _boundingColliderProp = serializedObject.FindProperty(nameof(boundingCollider));
                _entityDataSourceProp = serializedObject.FindProperty(nameof(entityDataSource));
                _factionIdField = new AutoCompleteField(
                    serializedObject.FindProperty(nameof(factionId)),
                    () => FactionsData.AllFactions.Select(f => f.Id)
                );
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
                _factionIdField.OnGUI();


                serializedObject.ApplyModifiedProperties();

                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    if (targets.Length > 1)
                    {
                        GUILayout.Label("Multi editing - error checking is not available", NonebGUIStyle.Hint);
                    }
                    else
                    {
                        var entity = (EditorEntity<T>)target;

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