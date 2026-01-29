using System;
using System.Linq;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Core.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils;
using Object = UnityEngine.Object;

namespace NonebNi.EditorTools.CustomDrawers
{
    [CustomEditor(typeof(Object), true, isFallback = true)]
    [CanEditMultipleObjects]
    public class NonebUniversalInspector : Editor
    {
        private (MethodInfo method, CallOnEditorEnabledAttribute attribute)[] _calledOnEnabledMethod = Array.Empty<(MethodInfo method, CallOnEditorEnabledAttribute attribute)>();
        private MethodInfo[] _buttonMethod = Array.Empty<MethodInfo>();
        private NonebGUIDrawer _editorDataDrawer = null!;
        private NonebGUIDrawer _mainDrawer = null!;
        private Object _self = null!;

        private void OnEnable()
        {
            _mainDrawer = new (serializedObject);
            _editorDataDrawer = new (EditorData.instance);
            _self = target;

            _buttonMethod = ReflectionUtils.GetMethodsByAttribute<ButtonAttribute>(target.GetType()).Select(t => t.method).ToArray();
            CallOnEditorEnabled();
        }

        private void CallOnEditorEnabled()
        {
            if (Application.isPlaying) return;

            _calledOnEnabledMethod = ReflectionUtils.GetMethodsByAttribute<CallOnEditorEnabledAttribute>(target.GetType()).ToArray();
            foreach (var (method, attribute) in _calledOnEnabledMethod) method.Invoke(_self, attribute.Parameters);
        }

        public override void OnInspectorGUI()
        {
            if (_self.GetType().GetAttribute<NonebUniversalEditorAttribute>(false) == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _mainDrawer.Update();
            _mainDrawer.DrawNonebInspector(this);
            _mainDrawer.Apply();

            _editorDataDrawer.Update();
            DrawButtonsMethod();
            DrawUIStacks();
            DrawCalledOnEnableInitMethod();
            _editorDataDrawer.Apply();

            Repaint();
        }

        private void DrawButtonsMethod()
        {
            if (!_buttonMethod.Any()) return;

            using (_editorDataDrawer.BoxScope())
            using (_editorDataDrawer.FlowLayoutScope())
            {
                foreach (var method in _buttonMethod)
                {
                    if (_editorDataDrawer.DrawButton(method.Name))
                    {
                        method.Invoke(_self, Array.Empty<object>());
                    }
                }
            }
        }

        private void DrawCalledOnEnableInitMethod()
        {
            if (!_calledOnEnabledMethod.Any()) return;

            using (_editorDataDrawer.BoxScope())
            {
                if (_editorDataDrawer.Foldout("Auto Called Init Method"))
                    foreach (var (method, attribute) in _calledOnEnabledMethod)
                    {
                        var paramLists = string.Join(",", attribute.Parameters.Select(p => p.ToString()));
                        _editorDataDrawer.DrawLabel($"{method.Name}({paramLists})");
                    }
            }
        }

        private void DrawUIStacks()
        {
            void DrawStack(UIStack? stack, string stackName, string idPrefix = "")
            {
                using (_editorDataDrawer.BoxScope())
                {
                    if (stack == null)
                    {
                        _editorDataDrawer.DrawLabel($"{stackName} is null");
                        return;
                    }

                    var stackFoldoutId = $"{idPrefix}.{stackName}";
                    if (_editorDataDrawer.Foldout(stackName, stackFoldoutId))
                        using (_editorDataDrawer.IndentScope())
                        {
                            using (_editorDataDrawer.HorizontalScope())
                            {
                                //TODO:handle substack.
                                using (_editorDataDrawer.DisabledScope(EditorData.instance.viewParameter == null))
                                {
                                    if (_editorDataDrawer.DrawButton("Push")) stack.Push(EditorData.instance.viewParameter!).Forget();
                                    if (_editorDataDrawer.DrawButton("Replace")) stack.ReplaceCurrent(EditorData.instance.viewParameter!).Forget();
                                }

                                if (_editorDataDrawer.DrawButton("Pop")) stack.Pop().Forget();
                                if (_editorDataDrawer.DrawButton("Make SubStack")) stack.GetSubStack(EditorData.instance.subStackName);
                            }

                            var requestId = $"{stackFoldoutId}.requests";
                            if (_editorDataDrawer.Foldout("Requests", requestId))
                                using (_editorDataDrawer.IndentScope())
                                {
                                    var infos = stack.RequestsInfo.ToArray();
                                    if (!infos.Any())
                                        _editorDataDrawer.DrawHint("No processing requests");
                                    else
                                        for (var i = 0; i < infos.Length; i++)
                                        {
                                            var info = infos[i];
                                            var infoText = $"{i}: {info.OperationName}";
                                            if (info.ParameterViewName != null) infoText += $" with {info.ParameterViewName}";

                                            _editorDataDrawer.DrawLabel(infoText);
                                        }
                                }

                            _editorDataDrawer.DrawHeader("Views");
                            var nonebViews = stack.GetViews().ToArray();
                            if (!nonebViews.Any())
                                _editorDataDrawer.DrawHint("No views in the stack");
                            else
                                for (var i = 0; i < nonebViews.Length; i++)
                                {
                                    var view = nonebViews[i];
                                    _editorDataDrawer.DrawLabel($"{i}: {view.Name}");
                                }

                            using (_editorDataDrawer.IndentScope())
                            {
                                foreach (var (subStackName, subStack) in stack.GetSubStacks()) DrawStack(subStack, subStackName, stackFoldoutId);
                            }
                        }
                }
            }

            var stacksField = _self.GetType().GetFieldsOfType<UIStack>().ToArray();
            if (!stacksField.Any()) return;

            if (!_editorDataDrawer.Foldout("UI Stacks")) return;

            using (_editorDataDrawer.DisabledScope(!Application.isPlaying))
            {
                using (_editorDataDrawer.BoxScope("UI Stacks Debug"))
                {
                    using (_editorDataDrawer.HorizontalScope())
                    {
                        _editorDataDrawer.DrawProperty(nameof(EditorData.viewParameter), isCompact: true);
                        _editorDataDrawer.DrawProperty(nameof(EditorData.subStackName), isCompact: true);
                    }
                }

                foreach (var fi in stacksField)
                {
                    var stack = fi.GetValue(_self) as UIStack;
                    var displayName = ObjectNames.NicifyVariableName(fi.Name);
                    DrawStack(stack, displayName);
                }
            }
        }

        public class EditorData : ScriptableSingleton<EditorData>
        {
            public NonebViewBehaviour? viewParameter;
            public string subStackName = string.Empty;
        }
    }
}