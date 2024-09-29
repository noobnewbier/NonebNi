using System.Linq;
using NonebNi.Ui.Common.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    internal class AnimatorStateDrawer : PropertyDrawer
    {
        private static SerializedObject? _lastSerializedObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var typedAttribute = (AnimatorStateAttribute)attribute;
            RefreshCache();

            var animator = typedAttribute.UseRootObjectField ?
                property.serializedObject.FindProperty(typedAttribute.AnimatorName).objectReferenceValue as Animator :
                NonebEditorUtils.FindPropertyObjectReferenceInSameDepth<Animator>(property, typedAttribute.AnimatorName);
            var animatorRuntimeAnimatorController = animator != null ?
                animator.runtimeAnimatorController :
                null;
            if (animator == null || animatorRuntimeAnimatorController == null)
            {
                GUI.Label(position, $"{property.name} : Referenced animator is null", NonebGUIStyle.Error);
            }
            else
            {
                var paramTable = AnimatorInfoCache.GetParamTable(animatorRuntimeAnimatorController);
                var targetLayer = typedAttribute.TargetLayerName == null ?
                    0 :
                    NonebEditorUtils.FindPropertyIntInSameDepth(property, typedAttribute.TargetLayerName);
                var states = paramTable.GetStates(targetLayer);
                NonebEditorGUI.ShowStringPopup(
                    position,
                    property,
                    $"{label.text}",
                    states
                );

                if (!states.Contains(property.stringValue)) property.stringValue = states.FirstOrDefault() ?? string.Empty;
            }

            EditorGUI.EndProperty();

            void RefreshCache()
            {
                if (_lastSerializedObject != property.serializedObject)
                {
                    _lastSerializedObject = property.serializedObject;
                    AnimatorInfoCache.ClearData();
                }
            }
        }
    }
}