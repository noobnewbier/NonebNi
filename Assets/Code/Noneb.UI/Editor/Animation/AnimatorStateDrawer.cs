using System.Linq;
using Noneb.UI.Animation.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
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

            var animator = EditorTimeAnimatorFinder.FindForInspector(property, typedAttribute.AnimatorName, typedAttribute.UseRootObjectField);
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