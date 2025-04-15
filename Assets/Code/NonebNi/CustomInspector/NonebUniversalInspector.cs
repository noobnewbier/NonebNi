using System.Linq;
using Cysharp.Threading.Tasks;
using Noneb.UI.View;
using NonebNi.Ui.Attributes;
using UnityEditor;
using UnityEngine;
using UnityUtils;

namespace NonebNi.CustomInspector
{
    [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
    public class NonebUniversalInspector : Editor
    {
        private NonebGUIDrawer _editorDataDrawer = null!;
        private NonebGUIDrawer _mainDrawer = null!;
        private MonoBehaviour _self = null!;

        private void OnEnable()
        {
            _mainDrawer = new NonebGUIDrawer(serializedObject);
            _editorDataDrawer = new NonebGUIDrawer(new SerializedObject(EditorData.instance));
            _self = (MonoBehaviour)target;
        }

        public override void OnInspectorGUI()
        {
            if (_self.GetType().GetAttribute<NonebUniversalEditorAttribute>(false) == null)
            {
                base.OnInspectorGUI();
                return;
            }

            _mainDrawer.Update();
            _mainDrawer.DrawDefaultInspector(this);
            _mainDrawer.Apply();

            _editorDataDrawer.Update();
            DrawUIStacks();
            _editorDataDrawer.Apply();

            Repaint();
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