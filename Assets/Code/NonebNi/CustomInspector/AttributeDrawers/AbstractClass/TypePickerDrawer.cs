using System;
using System.Collections.Generic;
using NonebNi.CustomInspector.AbstractClass;
using NonebNi.Ui.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.AttributeDrawers.AbstractClass
{
    [CustomPropertyDrawer(typeof(TypePickerAttribute))]
    public class TypePickerDrawer : PropertyDrawer
    {
        private static readonly Dictionary<SerializedObject, Dictionary<string, TypePicker>> PickersCache = new();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region Init

            var type = fieldInfo.FieldType;
            var typePicker = GetOrCreatePicker(property, type, label);

            #endregion

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

            typePicker.Draw(pickerRect, property);


            if (property.isExpanded)
                using (new EditorGUI.IndentLevelScope())
                {
                    var objectRect = pickerRect;
                    objectRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    objectRect.height = EditorGUIUtility.singleLineHeight;
                    NonebEditorUtils.DrawDefaultPropertyWithoutFoldout(objectRect, property);
                }
        }

        private static TypePicker GetOrCreatePicker(SerializedProperty property, Type type, GUIContent label)
        {
            if (!PickersCache.TryGetValue(property.serializedObject, out var cache)) PickersCache[property.serializedObject] = cache = new Dictionary<string, TypePicker>();

            if (!cache.TryGetValue(property.propertyPath, out var picker)) cache[property.propertyPath] = picker = new TypePicker(type, label);

            return picker;
        }
    }
}