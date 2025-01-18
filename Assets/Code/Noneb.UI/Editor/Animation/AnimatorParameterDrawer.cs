using Noneb.UI.Animation.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    internal class AnimatorParameterDrawer : PropertyDrawer
    {
        private static SerializedObject? _lastSerializedObject;
        private AnyTypeAnimatorParameterPicker? _parameterPicker;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var typedAttribute = (AnimatorParameterAttribute)attribute;
            RefreshCache();

            using (var scope = new EditorGUI.PropertyScope(position, label, property))
            {
                var (animator, controller) = GetAnimatorAndController(property, typedAttribute.AnimatorName, typedAttribute.UseRootObjectField);
                if (animator == null || controller == null)
                    GUI.Label(position, $"{property.name} : Referenced animator is null", NonebGUIStyle.Error);
                else if (typedAttribute.ParameterType.HasValue)
                    DrawSimpleDrawer(position, property, scope.content, typedAttribute.ParameterType.Value, controller);
                else
                    DrawAdvancedDrawer(position, property, label, controller);
            }

            return;

            void RefreshCache()
            {
                if (_lastSerializedObject != property.serializedObject)
                {
                    _lastSerializedObject = property.serializedObject;
                    AnimatorInfoCache.ClearData();
                }
            }
        }

        private static (Animator? animator, RuntimeAnimatorController? controller) GetAnimatorAndController(
            SerializedProperty property,
            string animatorName,
            bool useRootObjectField)
        {
            var animator = EditorTimeAnimatorFinder.FindForInspector(property, animatorName, useRootObjectField);
            var controller = animator != null ?
                animator.runtimeAnimatorController :
                null;

            return (animator, controller);
        }

        private void DrawAdvancedDrawer(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            RuntimeAnimatorController controller)
        {
            _parameterPicker ??= new AnyTypeAnimatorParameterPicker(controller, label);
            _parameterPicker.Draw(position, property);
        }

        private static void DrawSimpleDrawer(
            Rect position,
            SerializedProperty property,
            GUIContent label,
            AnimatorControllerParameterType parameterType,
            RuntimeAnimatorController animatorController)
        {
            var paramTable = AnimatorInfoCache.GetParamTable(animatorController);
            var parameters = paramTable.GetParameters(parameterType);
            NonebEditorGUI.ShowStringPopup(
                position,
                property,
                $"{label.text} ({parameterType})",
                parameters
            );
        }
    }
}