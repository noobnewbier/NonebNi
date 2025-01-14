using NonebNi.Ui.Common.Attributes;
using Unity.Logging;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    [CustomPropertyDrawer(typeof(AnimatorLayerAttribute))]
    internal class AnimatorLayerDrawer : PropertyDrawer
    {
        private static SerializedObject? _lastSerializedObject;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var typedAttribute = (AnimatorLayerAttribute)attribute;
            RefreshCache();

            Animator? animator;
            if (typedAttribute.UseRootObjectField)
            {
                if (string.IsNullOrEmpty(typedAttribute.AnimatorName))
                    animator = property.serializedObject.FindPropertyOfTypeAtRoot<Animator>();
                else
                    animator = property.serializedObject.FindProperty(typedAttribute.AnimatorName).objectReferenceValue as Animator;
            }
            else
            {
                if (string.IsNullOrEmpty(typedAttribute.AnimatorName))
                {
                    animator = null;
                    Log.Error(
                        "We cannot find animator automatically when you are not referencing the root type, you must define animator name before I decide to implement this."
                    );
                }
                else
                {
                    animator = NonebEditorUtils.FindPropertyObjectReferenceInSameDepth<Animator>(property, typedAttribute.AnimatorName);
                }
            }

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