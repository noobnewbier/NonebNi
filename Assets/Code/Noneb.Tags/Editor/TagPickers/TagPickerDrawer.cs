using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.Tags.Editor.TagPickers
{
    //todo: why the fuck do I even have this
    public abstract class TagPickerDrawer<T> : PropertyDrawer
    {
        private readonly TagPicker _tagPicker = new ();

        //todo: static -> domain reload + new gameplaytag....
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position.height = EditorGUIUtility.singleLineHeight;
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
            }

            _tagPicker.Draw(pickerRect, label, property);

            if (property.isExpanded)
                using (new EditorGUI.IndentLevelScope())
                {
                    var objectRect = pickerRect;
                    objectRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    objectRect.height = EditorGUIUtility.singleLineHeight;
                    NonebEditorGUI.DrawDefaultPropertyWithoutFoldout(objectRect, property);
                }
        }
    }
}