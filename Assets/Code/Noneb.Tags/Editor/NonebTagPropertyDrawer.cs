using Noneb.Tags.Editor.TagPickers;
using Noneb.Tags.Runtime;
using UnityEditor;
using UnityEngine;

namespace Noneb.Tags.Editor
{
    [CustomPropertyDrawer(typeof(NonebTag))]
    public class NonebTagPropertyDrawer : PropertyDrawer
    {
        private readonly TagPicker _tagPicker = new ();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                _tagPicker.Draw(position, label, property);
            }
        }
    }
}