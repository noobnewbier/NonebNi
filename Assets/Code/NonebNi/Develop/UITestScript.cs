using System;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.CustomInspector;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Develop
{
    //TODO: how should scene manangement with the UI state management
    public class UITestScript : MonoBehaviour
    {
        [SerializeField] private GameObject root = null!;
        private UIStack _stack = null!;

        private void Awake()
        {
            _stack = new UIStack(root);
        }

#if UNITY_EDITOR

        //TODO: warning get rid
        [Serializable]
        public class EditorData
        {
            public NonebViewBehaviour? viewToPush;
            public string subStackName = string.Empty;
        }

        [SerializeField] private EditorData editorData = new();


        [CustomEditor(typeof(UITestScript))]
        private class InsideEditor : Editor
        {
            private NonebGUIDrawer _drawer = null!;
            private UITestScript _self = null!;

            private void OnEnable()
            {
                _drawer = new NonebGUIDrawer(serializedObject);
                _self = (UITestScript)target;
            }

            public override void OnInspectorGUI()
            {
                _drawer.Update();

                _drawer.DrawDefaultInspector(this);

                _drawer.DrawEditorDataProperty(nameof(EditorData.viewToPush));
                _drawer.DrawEditorDataProperty(nameof(EditorData.subStackName));

                DrawStack(_self._stack, "Root");

                _drawer.Apply();
                Repaint();
            }

            private void DrawStack(UIStack? stack, string stackName, string idPrefix = "")
            {
                using (_drawer.BoxScope())
                {
                    if (stack == null)
                    {
                        _drawer.DrawLabel($"Null Stack - {stackName}");
                        return;
                    }

                    var id = $"{idPrefix}.{stackName}";

                    if (_drawer.Foldout(stackName, id))
                    {
                        using (_drawer.HorizontalScope())
                        {
                            //TODO:handle substack.
                            using (_drawer.DisabledScope(_self.editorData.viewToPush == null))
                            {
                                if (_drawer.DrawButton("Push")) stack.Push(_self.editorData.viewToPush!).Forget();

                                if (_drawer.DrawButton("Pop")) stack.Pop().Forget();

                                if (_drawer.DrawButton("Make SubStack")) stack.GetSubStack(_self.editorData.subStackName);
                            }
                        }

                        foreach (var view in stack.GetViews()) _drawer.DrawLabel(view.Name);

                        using (_drawer.IndentScope())
                        {
                            foreach (var (subStackName, subStack) in stack.GetSubStacks()) DrawStack(subStack, subStackName, id);
                        }
                    }
                }
            }
        }

#endif
    }
}