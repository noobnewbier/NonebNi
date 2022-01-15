using NonebNi.Ui.Common.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(AnimatorLayerAttribute))]
    internal class AnimatorLayerDrawer : PropertyDrawer
    {
        private static SerializedObject? _lastSerializedObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            AnimatorLayerAttribute typedAttribute = (AnimatorLayerAttribute)attribute;
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
                var layers = paramTable.GetLayers();
                var newIndex = NonebEditorGUI.ShowStringPopup(
                    position,
                    layers[property.intValue],
                    label.text,
                    layers
                );

                if (newIndex != property.intValue) property.intValue = newIndex;
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