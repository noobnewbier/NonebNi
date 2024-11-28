using NonebNi.CustomInspector.AttributeDrawers;
using NonebNi.Ui.Animation;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomPropertyDrawer(typeof(AnimationData))]
    public class AnimationDataDrawer : PropertyDrawer
    {
        private SerializedProperty? _finishAnimLayerProperty;
        private SerializedProperty? _finishAnimStateProperty;
        private SerializedProperty? _nameProperty;
        private AnyTypeAnimatorParameterPicker? _parameterPicker;
        private SerializedProperty? _typeProperty;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region Init

            _parameterPicker ??= CreateNewPicker(property, label);
            _nameProperty ??= property.NFindPropertyRelative(nameof(AnimationData.Name));
            _typeProperty ??= property.NFindPropertyRelative("type");
            _finishAnimStateProperty ??= property.NFindPropertyRelative(nameof(AnimationData.FinishAnimState));
            _finishAnimLayerProperty ??= property.NFindPropertyRelative(nameof(AnimationData.FinishAnimLayerIndex));

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
                var foldoutRect = position;
                foldoutRect.height = EditorGUIUtility.singleLineHeight;
                foldoutRect.width = EditorGUIUtility.labelWidth;
                property.isExpanded = EditorGUI.Foldout(foldoutRect, property.isExpanded, GUIContent.none);

                var pickerRect = position;
                pickerRect.x += NonebEditorUtils.FoldoutIconWidth;
                _parameterPicker.Draw(pickerRect, _nameProperty, _typeProperty);
            }

            var paramType = _typeProperty?.GetEnumValueFromProperty<AnimatorControllerParameterType>();
            var haveStateTransition = paramType is AnimatorControllerParameterType.Bool or AnimatorControllerParameterType.Trigger;

            if (property.isExpanded)
                using (new EditorGUI.IndentLevelScope())
                using (new EditorGUI.DisabledScope(!haveStateTransition))
                {
                    position = EditorGUI.IndentedRect(position);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, _finishAnimLayerProperty);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(position, _finishAnimStateProperty);
                }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        private static AnyTypeAnimatorParameterPicker? CreateNewPicker(SerializedProperty property, GUIContent label)
        {
            var controller = property.serializedObject.FindPropertyOfTypeAtRoot<Animator>()?.runtimeAnimatorController;
            if (controller == null) return null;

            var newPicker = new AnyTypeAnimatorParameterPicker(controller, label);
            return newPicker;
        }
    }
}