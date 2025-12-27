using System;
using System.Collections.Generic;
using NonebNi.Core.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.AttributeDrawers
{
    [CustomPropertyDrawer(typeof(AutoCompleteFieldAttribute))]
    public class AutoCompleteFieldDrawer : PropertyDrawer
    {
        //todo: finish this
        private static readonly Dictionary<SerializedObject, Dictionary<SerializedProperty, AutoCompleteField>> DrawerCache = new ();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUI.GetPropertyHeight(property);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            #region Init

            var typedAttribute = attribute as AutoCompleteFieldAttribute;
            var fieldDrawer = GetOrCreateField(property, typedAttribute, label);

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

            fieldDrawer.OnGUI(pickerRect);

            if (property.isExpanded)
                using (new EditorGUI.IndentLevelScope())
                {
                    var objectRect = pickerRect;
                    objectRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    objectRect.height = EditorGUIUtility.singleLineHeight;
                    NonebEditorGUI.DrawDefaultPropertyWithoutFoldout(objectRect, property);
                }
        }

        //todo: this doesn't flag malformed input -> tag doesn't exist doesn't go red. 
        private static AutoCompleteField GetOrCreateField(SerializedProperty property, AutoCompleteFieldAttribute? attribute, GUIContent label)
        {
            if (attribute == null) return new (_ => { }, _ => { }, Array.Empty<string>, new ("ERR: FAILED INIT"), "NOT FUNCTIONING");

            if (!DrawerCache.TryGetValue(property.serializedObject, out var cache)) DrawerCache[property.serializedObject] = cache = new ();

            if (!cache.TryGetValue(property, out var field)) cache[property] = field = new (property, attribute.OptionsFactory, label: label);

            return field;
        }
    }
}