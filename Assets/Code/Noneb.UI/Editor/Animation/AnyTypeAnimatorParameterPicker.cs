using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtils;
using UnityUtils.Editor;

namespace Noneb.UI.Editor.Animation
{
    /// <summary>
    /// If you are reading this, and know of a better way to do this, please enlighten me as it is gross.
    /// </summary>
    public class AnyTypeAnimatorParameterPicker
    {
        private readonly RuntimeAnimatorController _controller;
        private readonly GUIContent _label;

        private string _currentParamName = string.Empty;
        private AnimatorControllerParameterType? _currentParamType;
        private AnyTypeAnimatorParameterDropdown? _dropdown;

        //Essential to detect change
        private string _newParamName = string.Empty;
        private AnimatorControllerParameterType? _newParamType;

        public AnyTypeAnimatorParameterPicker(RuntimeAnimatorController controller) : this(controller, GUIContent.none) { }

        public AnyTypeAnimatorParameterPicker(RuntimeAnimatorController controller, string label) : this(controller, new GUIContent(label)) { }

        public AnyTypeAnimatorParameterPicker(RuntimeAnimatorController controller, GUIContent label)
        {
            _label = label;
            if (!string.IsNullOrEmpty(_label.text)) _label.text = $"{_label.text}(Any)";

            _controller = controller;
        }

        public (string paramName, AnimatorControllerParameterType? type) Draw(Rect position, string paramName, AnimatorControllerParameterType? paramType)
        {
            InitParameterNames(paramName, paramType);

            if (_dropdown == null)
            {
                _dropdown = new AnyTypeAnimatorParameterDropdown(_controller, new AdvancedDropdownState());
                _dropdown.NewParamSelected += arg =>
                {
                    _newParamName = arg?.paramName ?? string.Empty;
                    _newParamType = arg?.type;
                };
            }

            var pickerPos = position;
            if (_label == GUIContent.none || !string.IsNullOrEmpty(_label.text) && _label.image == null)
            {
                pickerPos.width = EditorGUIUtility.labelWidth;
                pickerPos.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(pickerPos, _label);

                pickerPos.x += pickerPos.width;
                pickerPos.width = position.width - pickerPos.width;
            }

            if (GUI.Button(pickerPos, _currentParamName)) _dropdown.Show(position);

            if (_currentParamName != _newParamName || _currentParamType != _newParamType)
            {
                _currentParamName = _newParamName;
                _currentParamType = _newParamType;

                GUI.changed = true;
            }

            return (_currentParamName, _currentParamType);
        }

        public void Draw(Rect position, SerializedProperty nameProperty, SerializedProperty? typeProperty = null)
        {
            var isExpectedPropertyType = CheckIsPropertyTypeExpected();
            if (!isExpectedPropertyType) GUI.Label(position, "Unexpected type getting thrown in.", NonebGUIStyle.Error);

            using var changeCheck = new EditorGUI.ChangeCheckScope();
            var result = Draw(position, nameProperty.stringValue, null);
            if (!changeCheck.changed) return;

            var (paramName, type) = result;
            var hasValueChanges = AssignNamePropertyChanges();
            hasValueChanges |= AssignTypePropertyChanges();

            if (hasValueChanges) ApplyModifiedProperties();

            return;

            bool AssignTypePropertyChanges()
            {
                if (typeProperty == null) return false;
                if (type == null) return false;

                var typeEnumIndex = type.Value.GetEnumIndex();
                if (typeProperty.enumValueIndex == typeEnumIndex) return false;

                typeProperty.enumValueIndex = typeEnumIndex;
                return true;
            }

            bool AssignNamePropertyChanges()
            {
                if (nameProperty.stringValue == (paramName ?? string.Empty)) return false;

                nameProperty.stringValue = paramName;
                return true;
            }

            void ApplyModifiedProperties()
            {
                nameProperty.serializedObject.ApplyModifiedProperties();

                //Avoid duplicated serialized object api call on the same instance.
                if (nameProperty.serializedObject != typeProperty?.serializedObject)
                    typeProperty?.serializedObject.ApplyModifiedProperties();
            }

            bool CheckIsPropertyTypeExpected()
            {
                if (nameProperty.propertyType != SerializedPropertyType.String) return false;
                if (typeProperty?.propertyType != SerializedPropertyType.Enum) return false;

                return true;
            }
        }


        private void InitParameterNames(string paramName, AnimatorControllerParameterType? paramType)
        {
            if (_currentParamName == paramName)
                if (paramType == null)
                {
                    paramType = InitParamTypeIfInvalid(paramName, paramType);
                    if (_currentParamType == paramType) return;
                }

            _currentParamName = paramName;
            _currentParamType = paramType;

            if (_currentParamType == null && !string.IsNullOrEmpty(_currentParamName)) _currentParamType = InitParamTypeIfInvalid(_currentParamName, _currentParamType);

            //Init at last to match what we currently have
            _newParamName = _currentParamName;
            _newParamType = _currentParamType;
        }

        private AnimatorControllerParameterType? InitParamTypeIfInvalid(string paramName, AnimatorControllerParameterType? paramType)
        {
            if (paramType == null) return null;

            var paramTable = AnimatorInfoCache.GetParamTable(_controller);
            var matchingType = paramTable.FindParameterTypeByName(paramName);

            return matchingType;
        }

        [InitializeOnLoad]
        public static class AnimatorWindowWatcher
        {
            private static EditorWindow? _lastFocusedWindow;

            static AnimatorWindowWatcher()
            {
                EditorApplication.update += Update;
            }

            private static bool IsAnimatorEditor(EditorWindow? window)
            {
                if (window == null) return false;

                return window.GetType().FullName == "UnityEditor.Graphs.AnimatorControllerTool";
            }

            private static void Update()
            {
                if (_lastFocusedWindow == EditorWindow.focusedWindow) return;

                // Refresh on exiting animator editing
                if (IsAnimatorEditor(_lastFocusedWindow)) AnimatorInfoCache.ClearData();

                // Refresh on entering animator editing
                if (IsAnimatorEditor(EditorWindow.focusedWindow)) AnimatorInfoCache.ClearData();

                _lastFocusedWindow = EditorWindow.focusedWindow;
            }
        }
    }
}