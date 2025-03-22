using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityUtils;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector
{
    public class NonebGUIDrawer
    {
        private readonly Dictionary<string, AutoCompleteField> _autoCompleteFieldsCache = new();
        private readonly Dictionary<string, bool> _foldoutStates = new();
        private readonly Dictionary<SerializedProperty, Dictionary<string, SerializedProperty?>> _nestedPropertyCache = new();
        private readonly Dictionary<string, SerializedProperty?> _propertyCache = new();
        private readonly Dictionary<string, ReorderableList> _reorderableListCache = new();
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

        public void DrawAutoCompleteProperty(string propertyPath, Func<IEnumerable<string>> autoCompleteOptionsFactory, string label = "")
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                DrawError($"Cannot find property with name == {propertyPath}");
                return;
            }

            var editedType = property.GetTargetType();
            if (editedType == null)
            {
                DrawError($"I have no idea what is a {propertyPath}");
                return;
            }

            if (editedType != typeof(string) && editedType.GetTypeWithinCollection() != typeof(string))
            {
                DrawError($"We can only work with strings here! And {propertyPath} a {editedType}");
                return;
            }

            if (string.IsNullOrEmpty(label)) label = property.displayName;
            if (editedType.IsArray || editedType.IsUnitySupportedCollection())
            {
                if (!_reorderableListCache.TryGetValue(propertyPath, out var reorderableList))
                {
                    _reorderableListCache[propertyPath] = reorderableList = new ReorderableList(property.serializedObject, property, true, false, true, true);
                    reorderableList.drawElementCallback = (rect, index, _, _) =>
                    {
                        var subElementPath = property.GetArrayElementAtIndex(index);
                        if (subElementPath.propertyPath == null)
                        {
                            DrawError(rect, $"Cannot find element at index == {index}");
                            return;
                        }

                        var subAutoCompleteField = GetOrCreateAutoCompleteField(subElementPath.propertyPath, autoCompleteOptionsFactory);
                        subAutoCompleteField.OnGUI(rect, false);
                    };
                    reorderableList.elementHeight = EditorGUIUtility.singleLineHeight;
                }

                // property.isExpanded = EditorGUILayout.Foldout(property.isExpanded, label, EditorStyles.foldoutHeader);
                DrawListFoldoutHeader(property, label);
                if (property.isExpanded) reorderableList.DoLayoutList();

                return;
            }

            var autoCompleteField = GetOrCreateAutoCompleteField(propertyPath, autoCompleteOptionsFactory);
            autoCompleteField.OnGUI();
        }

        private AutoCompleteField GetOrCreateAutoCompleteField(string propertyPath, Func<IEnumerable<string>> autoCompleteOptionsFactory)
        {
            var property = GetOrFindProperty(propertyPath);
            if (property == null)
            {
                const string errorFieldKey = ",InvalidPropertyPath,";
                if (!_autoCompleteFieldsCache.TryGetValue(errorFieldKey, out var errorField))
                    _autoCompleteFieldsCache[errorFieldKey] = errorField = new AutoCompleteField(
                        null,
                        null,
                        () => Array.Empty<string>(),
                        "ERROR_NO_PROPERTY",
                        "cannot find property - should neve get here!"
                    );

                return errorField;
            }

            if (!_autoCompleteFieldsCache.TryGetValue(propertyPath, out var autoCompleteField)) _autoCompleteFieldsCache[propertyPath] = autoCompleteField = new AutoCompleteField(property, autoCompleteOptionsFactory);

            return autoCompleteField;
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

        public void DrawListFoldoutHeader(SerializedProperty listProperty, string label)
        {
            //Copying from https://github.com/Unity-Technologies/UnityCsReference/blob/b1cf2a8251cce56190f455419eaa5513d5c8f609/Editor/Mono/Inspector/ReorderableListWrapper.cs#L209, with some tweaks to make everything compiles
            const float headerPadding = 3f; // todo: maybe handle this as well if not too hassle
            const float arraySizeWidth = 48f;
            const float defaultFoldoutHeaderHeight = 18;

            var style = EditorStyles.foldoutHeader;
            var content = new GUIContent(label);

            var rect = GUILayoutUtility.GetRect(content, style);
            var headerRect = new Rect(rect.x, rect.y, rect.width - arraySizeWidth, defaultFoldoutHeaderHeight);
            var sizeRect = new Rect(
                headerRect.xMax - Indent * EditorGUI.indentLevel,
                headerRect.y,
                arraySizeWidth + Indent * EditorGUI.indentLevel,
                defaultFoldoutHeaderHeight
            );

            if (!listProperty.isArray) DrawError(rect, "This is not even a list, wtf are you doing");

            listProperty.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(headerRect, listProperty.isExpanded, label);
            EditorGUI.EndFoldoutHeaderGroup();
            var sizeProperty = GetOrFindProperty(listProperty, "Array.size");
            EditorGUI.PropertyField(sizeRect, sizeProperty, GUIContent.none);
        }

        #region Indent

        private static readonly Lazy<PropertyInfo> IndentMethod = new(
            () =>
            {
                var method = typeof(EditorGUI).GetProperty("indent", BindingFlags.Static | BindingFlags.NonPublic);
                return method!;
            }
        );

        public static int Indent
        {
            get
            {
                var getValue = IndentMethod.Value.GetValue(null);
                if (getValue is not int indent) return EditorGUI.indentLevel * 15;

                return indent;
            }
        }

        #endregion
    }
}