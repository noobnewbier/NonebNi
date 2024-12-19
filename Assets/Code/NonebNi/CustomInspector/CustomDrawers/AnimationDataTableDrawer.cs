using NonebNi.Ui.Animation;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.CustomDrawers
{
    [CustomPropertyDrawer(typeof(AnimationDataTable))]
    public class AnimationDataTableDrawer : PropertyDrawer
    {
        private SerializedProperty? _animIdAndDataProperty;

        private void Init(SerializedProperty property)
        {
            _animIdAndDataProperty ??= property.NFindPropertyRelative("animIdAndData")!;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            EditorGUI.PropertyField(position, _animIdAndDataProperty, label);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);

            return EditorGUI.GetPropertyHeight(_animIdAndDataProperty, label);
        }
    }
}