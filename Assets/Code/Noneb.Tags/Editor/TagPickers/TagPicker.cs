using System.Linq;
using Noneb.Tags.Runtime;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace Noneb.Tags.Editor.TagPickers
{
    public class TagPicker
    {
        private readonly TagPickerDropdown _dropdown;
        private NonebTag _currentTag = NonebTag.None;
        private NonebTag _newTag = NonebTag.None;

        public TagPicker()
        {
            _dropdown = new ();
            _dropdown.NewTagSelected += arg => { _newTag = arg; };
        }

        public (bool changed, NonebTag tag) Draw(Rect position, GUIContent label, NonebTag tag)
        {
            _currentTag = tag;
            if (!string.IsNullOrEmpty(label.text)) label.text = $"{label.text}";

            var pickerPos = position;
            pickerPos.height = EditorGUIUtility.singleLineHeight;
            if (label == GUIContent.none || !string.IsNullOrEmpty(label.text) && label.image == null) pickerPos = EditorGUI.PrefixLabel(position, label);


            var hasError = !EditorNonebTagManager.GetAllTags().Contains(_currentTag);
            using (new NonebEditorGUI.ErrorColorScope(hasError))
            {
                if (GUI.Button(pickerPos, tag.DisplayName)) _dropdown.Show(position);
            }

            var changed = false;
            if (_currentTag != _newTag)
            {
                _currentTag = _newTag;
                changed = true;
                GUI.changed = true;
            }

            return (changed, _currentTag);
        }

        public void Draw(Rect position, GUIContent label, SerializedProperty fieldProperty)
        {
            var targetType = fieldProperty.GetTargetType();
            var isExpectedPropertyType = targetType == typeof(NonebTag);
            if (!isExpectedPropertyType) GUI.Label(position, $"Unexpected type getting thrown in {targetType}.", NonebGUIStyle.Error);

            var typedValue = fieldProperty.boxedValue as NonebTag ?? NonebTag.None;

            using var changeCheck = new EditorGUI.ChangeCheckScope();
            var result = Draw(position, label, typedValue);
            if (!changeCheck.changed) return;

            var hasValueChanges = AssignTypePropertyChanges();
            if (hasValueChanges) fieldProperty.serializedObject.ApplyModifiedProperties();

            return;

            bool AssignTypePropertyChanges()
            {
                if (!result.changed) return false;
                if (result.tag == null) return false;

                fieldProperty.boxedValue = result.tag;
                return true;
            }
        }
    }
}