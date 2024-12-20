using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector
{
    public class NonebGUIDrawer
    {
        private readonly Dictionary<string, SerializedProperty?> _cache = new();
        private readonly SerializedObject _serializedObject;

        public NonebGUIDrawer(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
        }

        public void DrawProperty(string propertyPath, string label = "")
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                DrawError($"Cannot find property with name == {propertyPath}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = property.displayName;
            EditorGUILayout.PropertyField(property, new GUIContent(label));
        }

        public bool DrawButton(string label) => GUILayout.Button(label);

        public void DrawError(string errorText)
        {
            GUILayout.Label(errorText, NonebGUIStyle.Error);
        }

        private SerializedProperty? GetOrFindProperty(string propertyName)
        {
            if (!_cache.TryGetValue(propertyName, out var targetProperty))
            {
                targetProperty = _serializedObject.FindProperty(propertyName);
                if (targetProperty == null)
                {
                    var backingFieldPath = NonebEditorUtils.GetPropertyBindingPath(propertyName);
                    targetProperty = _serializedObject.FindProperty(backingFieldPath);
                }

                _cache[propertyName] = targetProperty;
            }

            return targetProperty;
        }

        public bool Update() => _serializedObject.UpdateIfRequiredOrScript();

        public bool Apply() => _serializedObject.ApplyModifiedProperties();
    }
}