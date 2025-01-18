using Noneb.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationDataDrawer : PropertyDrawer
    {
        private SerializedProperty? _finishAnimLayerProperty;
        private SerializedProperty? _finishAnimStateProperty;
        private SerializedProperty? _isRootMotionProperty;
        private SerializedProperty? _nameProperty;

        private AnyTypeAnimatorParameterPicker? _parameterPicker;
        private SerializedProperty? _targetBoolValueProperty;
        private SerializedProperty? _targetNumericValueProperty;
        private SerializedProperty? _typeProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region Init

            _parameterPicker ??= CreateNewPicker(property, label)!;
            _nameProperty ??= property.NFindPropertyRelative(nameof(AnimationData.Name))!;
            _isRootMotionProperty ??= property.NFindPropertyRelative(nameof(AnimationData.IsRootMotion))!;
            _typeProperty ??= property.NFindPropertyRelative("type")!;
            _finishAnimStateProperty ??= property.NFindPropertyRelative(nameof(AnimationData.FinishAnimState))!;
            _finishAnimLayerProperty ??= property.NFindPropertyRelative(nameof(AnimationData.FinishAnimLayerIndex))!;
            _targetBoolValueProperty ??= property.NFindPropertyRelative(nameof(AnimationData.TargetBoolValue))!;
            _targetNumericValueProperty ??= property.NFindPropertyRelative(nameof(AnimationData.TargetNumericValue))!;

            #endregion

            position.height = EditorGUIUtility.singleLineHeight;
            if (_parameterPicker == null)
            {
                GUI.Label(position, "Unable to find animator, check your prefab setup.", NonebGUIStyle.Error);
            }
            else if (_nameProperty == null)
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

                _parameterPicker.Draw(pickerRect, _nameProperty, _typeProperty);
            }


            if (property.isExpanded)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    position = EditorGUI.IndentedRect(position);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, _isRootMotionProperty, true);

                    var paramType = _typeProperty?.GetEnumValueFromProperty<AnimatorControllerParameterType>();
                    var haveStateTransition = paramType is AnimatorControllerParameterType.Bool or AnimatorControllerParameterType.Trigger; //TODO: this feels like a mis-design
                    using (new EditorGUI.DisabledScope(!haveStateTransition))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, _finishAnimLayerProperty, true);

                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, _finishAnimStateProperty, true);
                    }

                    var isBoolType = paramType is AnimatorControllerParameterType.Bool;
                    using (new EditorGUI.DisabledScope(!isBoolType))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        EditorGUI.PropertyField(position, _targetBoolValueProperty, true);
                    }

                    var isNumericType = paramType is AnimatorControllerParameterType.Int or AnimatorControllerParameterType.Float;
                    using (new EditorGUI.DisabledScope(!isNumericType))
                    {
                        position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                        if (paramType is AnimatorControllerParameterType.Int)
                            using (var check = new EditorGUI.ChangeCheckScope())
                            {
                                _targetNumericValueProperty.floatValue = EditorGUI.IntField(position, (int)_targetNumericValueProperty.floatValue);

                                if (check.changed) _targetNumericValueProperty.serializedObject.ApplyModifiedProperties();
                            }
                        else
                            EditorGUI.PropertyField(position, _targetNumericValueProperty, true);
                    }
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 6 + EditorGUIUtility.standardVerticalSpacing * 5;
        }

        private static AnyTypeAnimatorParameterPicker? CreateNewPicker(SerializedProperty property, GUIContent label)
        {
            var animator = EditorTimeAnimatorFinder.FindForInspector(property, string.Empty, true);
            var controller = animator?.runtimeAnimatorController;
            if (controller == null) return null;

            var newPicker = new AnyTypeAnimatorParameterPicker(controller, label);
            return newPicker;
        }
    }
}