using System.Collections.Generic;
using Noneb.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationDataDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, AnyTypeAnimatorParameterPicker> PickersCache = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region Init

            var parameterPicker = GetOrCreatePicker(property, label);
            var nameProperty = property.NFindPropertyRelative(nameof(AnimationData.Name));
            var isRootMotionProperty = property.NFindPropertyRelative(nameof(AnimationData.IsRootMotion))!;
            var typeProperty = property.NFindPropertyRelative("type")!;
            var finishAnimStateProperty = property.NFindPropertyRelative(nameof(AnimationData.FinishAnimState))!;
            var finishAnimLayerProperty = property.NFindPropertyRelative(nameof(AnimationData.FinishAnimLayerIndex))!;
            var targetBoolValueProperty = property.NFindPropertyRelative(nameof(AnimationData.TargetBoolValue))!;
            var targetNumericValueProperty = property.NFindPropertyRelative(nameof(AnimationData.TargetNumericValue))!;

            #endregion

            position.height = EditorGUIUtility.singleLineHeight;
            if (parameterPicker == null)
            {
                GUI.Label(position, "Unable to find animator, check your prefab setup.", NonebGUIStyle.Error);
            }
            else if (nameProperty == null)
            {
                GUI.Label(position, "Unable to find all properties, something went wrong.", NonebGUIStyle.Error);
            }
            else
            {
                var pickerRect = position;
                var isWithinFoldout = property.IsArrayElement();
                if (isWithinFoldout)
                {
                    property.isExpanded = true;
                }
                else
                {
                    var foldoutRect = position;
                    foldoutRect.height = EditorGUIUtility.singleLineHeight;
                    foldoutRect.width = EditorGUIUtility.labelWidth;
                    property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);

                    pickerRect = NonebEditorUtils.ShiftRect(pickerRect, NonebEditorUtils.FoldoutIconWidth);
                }

                parameterPicker.Draw(pickerRect, nameProperty, typeProperty);
            }


            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    position = EditorGUI.IndentedRect(position);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, isRootMotionProperty, true);

                    var paramType = typeProperty?.GetEnumValueFromProperty<AnimatorControllerParameterType>();
                    var haveStateTransition = paramType is AnimatorControllerParameterType.Bool or AnimatorControllerParameterType.Trigger; //TODO: this feels like a mis-design
                    using (new EditorGUI.DisabledScope(!haveStateTransition))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, finishAnimLayerProperty, true);

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, finishAnimStateProperty, true);
                    }

                    var isBoolType = paramType is AnimatorControllerParameterType.Bool;
                    using (new EditorGUI.DisabledScope(!isBoolType))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, targetBoolValueProperty, true);
                    }

                    var isNumericType = paramType is AnimatorControllerParameterType.Int or AnimatorControllerParameterType.Float;
                    using (new EditorGUI.DisabledScope(!isNumericType))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        if (paramType is AnimatorControllerParameterType.Int)
                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                targetNumericValueProperty.floatValue = EditorGUI.IntField(position, (int)targetNumericValueProperty.floatValue);

                                if (check.changed) targetNumericValueProperty.serializedObject.ApplyModifiedProperties();
                            }
                        else
                            EditorGUI.PropertyField(position, targetNumericValueProperty, true);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5;
        }

        private static AnyTypeAnimatorParameterPicker? GetOrCreatePicker(SerializedProperty property, GUIContent label)
        {
            var animator = EditorTimeAnimatorFinder.FindForInspector(property, string.Empty, true);
            if (animator == null) return null;

            var controller = animator.runtimeAnimatorController;
            if (controller == null) return null;

            if (!PickersCache.TryGetValue(property.propertyPath, out var picker)) PickersCache[property.propertyPath] = picker = new AnyTypeAnimatorParameterPicker(controller, label);

            return picker;
        }
    }
}