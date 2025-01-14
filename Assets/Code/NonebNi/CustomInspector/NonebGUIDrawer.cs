using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector
{
    public class NonebGUIDrawer
    {
        private readonly Dictionary<string, bool> _foldoutStates = new();
        private readonly Dictionary<SerializedProperty, Dictionary<string, SerializedProperty?>> _nestedPropertyCache = new();
        private readonly Dictionary<string, SerializedProperty?> _propertyCache = new();
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

        public void DrawNestedProperty(string pathToObj, string relativePathToField, string label = "")
        {
            var objProperty = GetOrFindProperty(pathToObj);
            if (objProperty == null)
            {
                DrawError($"Cannot find property with name == {pathToObj}");
                return;
            }

            var fieldProperty = GetOrFindProperty(objProperty, relativePathToField);
            if (fieldProperty == null)
            {
                DrawError($"Cannot find property with name == {relativePathToField}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = fieldProperty.displayName;
            EditorGUILayout.PropertyField(fieldProperty, new GUIContent(label));
        }

        public void DrawEditorDataProperty(string dataFieldName, string label = "")
        {
            // I can/will regret later
            DrawNestedProperty("editorData", dataFieldName, label);
        }

        public bool DrawButton(string label) => GUILayout.Button(label);

        public void DrawError(string errorText)
        {
            GUILayout.Label(errorText, NonebGUIStyle.Error);
        }

        public void DrawHeader(string headerText)
        {
            // The inconsistent use of editor styles here feels weird
            GUILayout.Label(headerText, EditorStyles.boldLabel);
            GUILayout.Space(EditorGUIUtility.standardVerticalSpacing);
        }

        public void DrawLabel(string label)
        {
            GUILayout.Label(label, NonebGUIStyle.Normal);
        }

        public bool DrawDefaultInspector(Editor editor) => DoDrawDefaultInspector(editor);

        private static bool DoDrawDefaultInspector(SerializedObject obj)
        {
            EditorGUI.BeginChangeCheck();
            obj.UpdateIfRequiredOrScript();
            var iterator = obj.GetIterator();
            for (var enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false)
            {
                if (iterator.propertyPath.StartsWith("editorData")) continue;

                using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
                {
                    EditorGUILayout.PropertyField(iterator, true);
                }
            }

            obj.ApplyModifiedProperties();
            return EditorGUI.EndChangeCheck();
        }

        private static bool DoDrawDefaultInspector(Editor editor)
        {
            bool flag;
            using (new LocalizationGroup(editor.target))
            {
                flag = DoDrawDefaultInspector(editor.serializedObject);

                /*
                 * Commented out as we don't need it for our custom inspector,
                 * we can regret and use reflection(these are all internal) later if it becomes a problem
                 */
                // MonoBehaviour target = this.target as MonoBehaviour;
                // if ((UnityEngine.Object) target == (UnityEngine.Object) null || !AudioUtil.HasAudioCallback(target) || AudioUtil.GetCustomFilterChannelCount(target) <= 0)
                //     return flag;
                // if (this.m_AudioFilterGUI == null)
                //     this.m_AudioFilterGUI = new AudioFilterGUI();
                // this.m_AudioFilterGUI.DrawAudioFilterGUI(target);
            }

            return flag;
        }

        private SerializedProperty? GetOrFindProperty(string propertyName)
        {
            if (!_propertyCache.TryGetValue(propertyName, out var targetProperty))
            {
                targetProperty = _serializedObject.FindProperty(propertyName);
                if (targetProperty == null)
                {
                    var backingFieldPath = NonebEditorUtils.GetPropertyBindingPath(propertyName);
                    targetProperty = _serializedObject.FindProperty(backingFieldPath);
                }

                _propertyCache[propertyName] = targetProperty;
            }

            return targetProperty;
        }

        private SerializedProperty? GetOrFindProperty(SerializedProperty nestedProperty, string propertyName)
        {
            if (!_nestedPropertyCache.TryGetValue(nestedProperty, out var cache)) _nestedPropertyCache[nestedProperty] = cache = new Dictionary<string, SerializedProperty?>();

            if (!cache.TryGetValue(propertyName, out var targetProperty))
            {
                targetProperty = nestedProperty.FindPropertyRelative(propertyName);
                if (targetProperty == null)
                {
                    var backingFieldPath = NonebEditorUtils.GetPropertyBindingPath(propertyName);
                    targetProperty = nestedProperty.FindPropertyRelative(backingFieldPath);
                }

                cache[propertyName] = targetProperty;
            }

            return targetProperty;
        }

        public bool Foldout(string label) => Foldout(label, label);

        public bool Foldout(string label, string id)
        {
            if (!_foldoutStates.TryGetValue(id, out var state)) _foldoutStates[id] = state = false;

            return _foldoutStates[id] = EditorGUILayout.Foldout(state, label, EditorStyles.foldoutHeader);
        }

        public IDisposable HorizontalScope() => new EditorGUILayout.HorizontalScope();

        public IDisposable VerticalScope() => new EditorGUILayout.VerticalScope();

        public IDisposable BoxScope()
        {
            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.padding.left *= 4;

            return new EditorGUILayout.VerticalScope(boxStyle);
        }

        public IDisposable DisabledScope(bool isDisabled) => new EditorGUI.DisabledScope(isDisabled);

        public IDisposable IndentScope(int indent = 1) => new EditorGUI.IndentLevelScope(indent);

        public bool Update() => _serializedObject.UpdateIfRequiredOrScript();

        public bool Apply() => _serializedObject.ApplyModifiedProperties();
    }
}