using System;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtils;
using UnityUtils.Editor;

namespace NonebNi.CustomInspector.AbstractClass
{
    /// <summary>
    /// If you are reading this, and know of a better way to do this, please enlighten me as it is gross.
    ///
    /// Note:
    /// can adapt to a new way of doing it - this guy did a lot of leg work already?
    /// https://github.com/AlexeyTaranov/SerializeReferenceDropdown
    /// </summary>
    public class TypePicker
    {
        private readonly Type _baseType;

        private readonly TypePickerDropdown _dropdown;
        private readonly GUIContent _label;
        private Type? _currentType;

        //Essential to detect change
        private Type? _newType;

        public TypePicker(Type baseType) : this(baseType, GUIContent.none) { }

        public TypePicker(Type baseType, string label) : this(baseType, new GUIContent(label)) { }

        public TypePicker(Type baseType, GUIContent label)
        {
            _label = label;
            if (!string.IsNullOrEmpty(_label.text)) _label.text = $"{_label.text}(Any)";

            _baseType = baseType;

            _dropdown = new TypePickerDropdown(_baseType, new AdvancedDropdownState());
            _dropdown.NewTypeSelected += arg => { _newType = arg; };
        }

        public (bool changed, Type? type) Draw(Rect position, Type? paramType)
        {
            _currentType = paramType;
            var pickerPos = position;
            pickerPos.height = EditorGUIUtility.singleLineHeight;
            if (_label == GUIContent.none || !string.IsNullOrEmpty(_label.text) && _label.image == null) pickerPos = EditorGUI.PrefixLabel(position, _label);

            // default it to "something" sensible
            if (GUI.Button(pickerPos, _currentType?.Name ?? "Empty!")) _dropdown.Show(position);

            if (_currentType == null) _newType ??= _baseType.GetTypeHierarchy().Flatten().FirstOrDefault(t => t.IsConcrete());

            var changed = false;
            if (_currentType != _newType)
            {
                _currentType = _newType;
                changed = true;
                GUI.changed = true;
            }

            return (changed, _currentType);
        }

        public void Draw(Rect position, SerializedProperty fieldProperty)
        {
            var isExpectedPropertyType = fieldProperty.propertyType == SerializedPropertyType.ManagedReference;
            if (!isExpectedPropertyType) GUI.Label(position, "Unexpected type getting thrown in.", NonebGUIStyle.Error);

            using var changeCheck = new EditorGUI.ChangeCheckScope();
            var result = Draw(position, fieldProperty.boxedValue?.GetType());
            if (!changeCheck.changed) return;

            var hasValueChanges = AssignTypePropertyChanges();
            if (hasValueChanges) fieldProperty.serializedObject.ApplyModifiedProperties();

            return;

            bool AssignTypePropertyChanges()
            {
                if (!result.changed) return false;
                if (result.type == null) return false;

                var newInstance = result.type.CreateObjectWithLeastPossibleDependencies();
                fieldProperty.boxedValue = newInstance;
                return true;
            }
        }
    }
}