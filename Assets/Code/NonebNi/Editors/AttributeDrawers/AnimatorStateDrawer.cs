using NonebNi.Ui.Common.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.Editors.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(AnimatorStateAttribute))]
    internal class AnimatorStateDrawer : PropertyDrawer
    {
        private static SerializedObject? _lastSerializedObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            AnimatorStateAttribute typedAttribute = (AnimatorStateAttribute)attribute;
            RefreshCache();

            var animator =
                NonebEditorUtils.FindPropertyObjectReferenceInSameDepth<Animator>(property, typedAttribute.AnimatorName);
            var animatorRuntimeAnimatorController = animator != null ? animator.runtimeAnimatorController : null;
            if (animator == null || animatorRuntimeAnimatorController == null)
            {
                GUI.Label(position, $"{property.name} : Referenced animator is null", NonebGUIStyle.Error);
            }
            else
            {
                var paramTable = AnimatorInfoCache.GetParamTable(animatorRuntimeAnimatorController);
                var states = paramTable.GetStates(typedAttribute.StateLayerIndex);
                NonebEditorGUI.ShowStringPopup(
                    position,
                    property,
                    typedAttribute.StateLayerIndex == -1 ?
                        $"{label.text} (State)" :
                        $"{label.text}(State:{typedAttribute.StateLayerIndex})",
                    states
                );
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