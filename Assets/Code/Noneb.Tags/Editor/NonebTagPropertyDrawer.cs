using System.Collections.Generic;
using Noneb.Tags.Editor.TagPickers;
using Noneb.Tags.Runtime;
using UnityEditor;
using UnityEngine;

namespace Noneb.Tags.Editor
{
    [CustomPropertyDrawer(typeof(NonebTag))]
    public class NonebTagPropertyDrawer : PropertyDrawer
    {
        private static readonly Dictionary<string, TagPicker> PickersCache = new ();

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                GetOrCreatePicker(property).Draw(position, label, property);
            }
        }

        private static TagPicker GetOrCreatePicker(SerializedProperty property)
        {
            // note: might be better to differentiate with serialized object as well...?
            if (!PickersCache.TryGetValue(property.propertyPath, out var picker)) PickersCache[property.propertyPath] = picker = new ();

            return picker;
        }
    }
}