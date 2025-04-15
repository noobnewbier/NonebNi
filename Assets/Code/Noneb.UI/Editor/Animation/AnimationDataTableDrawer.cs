﻿using Noneb.UI.Animation;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    [CustomPropertyDrawer(typeof(AnimationDataTable))]
    public class AnimationDataTableDrawer : PropertyDrawer
    {
        private SerializedProperty? _animIdAndDataProperty;

        private void FindProperties(SerializedProperty property)
        {
            _animIdAndDataProperty = property.NFindPropertyRelative("animIdAndData")!;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            EditorGUI.PropertyField(position, _animIdAndDataProperty, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            FindProperties(property);

            return EditorGUI.GetPropertyHeight(_animIdAndDataProperty, label);
        }
    }
}