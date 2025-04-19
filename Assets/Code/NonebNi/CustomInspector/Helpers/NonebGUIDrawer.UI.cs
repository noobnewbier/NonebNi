using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector
{
    public partial class NonebGUIDrawer
    {
        private static readonly Lazy<PropertyInfo> IndentMethod = new(
            () =>
            {
                var method = typeof(EditorGUI).GetProperty("indent", BindingFlags.Static | BindingFlags.NonPublic);
                return method!;
            }
        );

        private readonly Dictionary<string, bool> _foldoutStates = new();

        public NonebGUIDrawer(SerializedObject serializedObject)
        {
            _serializedObject = serializedObject;
        }

        public static int Indent
        {
            get
            {
                var getValue = IndentMethod.Value.GetValue(null);
                if (getValue is not int indent) return EditorGUI.indentLevel * 15;

                return indent;
            }
        }


        //todo: maybe? https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerargumentexpressionattribute?view=net-6.0

        public bool DrawButton(string label) => GUILayout.Button(label);

        public void DrawError(string errorText)
        {
            GUILayout.Label(errorText, NonebGUIStyle.Error);
        }

        public void DrawError(Rect rect, string errorText)
        {
            GUI.Label(rect, errorText, NonebGUIStyle.Error);
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

        public void DrawHint(string label)
        {
            GUILayout.Label(label, NonebGUIStyle.Hint);
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

        public bool Foldout(string label) => Foldout(label, label);

        public bool Foldout(string label, string id)
        {
            if (!_foldoutStates.TryGetValue(id, out var state)) _foldoutStates[id] = state = false;

            return _foldoutStates[id] = EditorGUILayout.Foldout(state, label, EditorStyles.foldoutHeader);
        }

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
    }
}