using System.Collections.Generic;
using NonebNi.Core.Stats;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomPropertyDrawer(typeof(StatCost))]
    public class StatCostEditor : PropertyDrawer
    {
        private static readonly Dictionary<string, AutoCompleteField> FieldCache = new();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var statIdProp = property.NFindPropertyRelative("StatId");
            var costProp = property.NFindPropertyRelative("Cost");
            if (statIdProp == null || costProp == null)
            {
                GUILayout.Label("Cannot find property - you probably renamed stuff and didn't change the editor", NonebGUIStyle.Error);
                return;
            }

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, property.displayName);
            if (!property.isExpanded) return;

            using (new EditorGUI.IndentLevelScope())
            {
                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                var field = GetOrCreateAutoCompleteField(statIdProp);
                field.OnGUI(position, false);

                position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                EditorGUI.PropertyField(position, costProp);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight * 3 + EditorGUIUtility.standardVerticalSpacing * 2;
        }

        private static AutoCompleteField GetOrCreateAutoCompleteField(SerializedProperty property)
        {
            if (!FieldCache.TryGetValue(property.propertyPath, out var field)) FieldCache[property.propertyPath] = field = new AutoCompleteField(property, () => StatId.All);

            return field;
        }
    }
}