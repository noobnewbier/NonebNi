using System;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector
{
    public partial class NonebGUIDrawer
    {
        public IDisposable HorizontalScope() => new EditorGUILayout.HorizontalScope();
        public IDisposable VerticalScope() => new EditorGUILayout.VerticalScope();

        public IDisposable BoxScope(string heading = "")
        {
            var boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.padding.left *= 4;

            var verticalScope = new EditorGUILayout.VerticalScope(boxStyle);
            if (!string.IsNullOrEmpty(heading)) DrawHeader(heading);

            return verticalScope;
        }

        public IDisposable DisabledScope(bool isDisabled) => new EditorGUI.DisabledScope(isDisabled);
        public IDisposable IndentScope(int indent = 1) => new EditorGUI.IndentLevelScope(indent);

        public IDisposable HandlesColorScope(Color color) => new NonebEditorUtils.HandlesColorScope(color);
        public IDisposable GizmosColorScope(Color color) => new NonebEditorUtils.GizmosColorScope(color);
        public IDisposable EditorLabelWidthScope(string targetLabel) => new NonebEditorUtils.EditorLabelWidthScope(targetLabel);
        public IDisposable AssetDatabaseEditingScope() => new NonebEditorUtils.AssetDatabaseEditingScope();
    }
}