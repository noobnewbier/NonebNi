using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityUtils.Editor;

namespace NonebNi.EditorTools.AttributeDrawers
{
    /// <summary>
    /// Copied from Unity
    /// https://github.com/Unity-Technologies/UnityCsReference/blob/59b03b8a0f179c0b7e038178c90b6c80b340aa9f/Editor/Mono/Inspector/Core/ScriptAttributeGUI/Implementations/PropertyDrawers.cs#L463
    /// Make one alteration such that on IMGUI end it can support Vector3.
    /// Still can't believe I have to do this myself, but what can I say
    /// </summary>
    [CustomPropertyDrawer(typeof(DelayedAttribute))]
    public class NDelayedDrawer : PropertyDrawer
    {
        private static readonly string InvalidTypeMessage = L10n.Tr("Dear Unity haven't supported this, time to do it ourself");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    EditorGUI.DelayedFloatField(position, property, label);
                    break;
                case SerializedPropertyType.Integer:
                    EditorGUI.DelayedIntField(position, property, label);
                    break;
                case SerializedPropertyType.String:
                    EditorGUI.DelayedTextField(position, property, label);
                    break;
                case SerializedPropertyType.Vector3:
                    NonebEditorGUI.DelayedVector3Field(position, property, label);
                    break;
                
                case SerializedPropertyType.Vector2:
                    NonebEditorGUI.DelayedVector2Field(position, property, label);
                    break;

                default:
                    EditorGUI.LabelField(position, label.text, InvalidTypeMessage);
                    break;
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            BindableElement? newField = null;
            if (property.propertyType == SerializedPropertyType.Float)
            {
                if (property.type == "float")
                {
                    newField = new FloatField(preferredLabel);
                    ((TextInputBaseField<float>)newField).isDelayed = true;
                }
                else if (property.type == "double")
                {
                    newField = new DoubleField(preferredLabel);
                    ((TextInputBaseField<double>)newField).isDelayed = true;
                }
            }
            else if (property.propertyType == SerializedPropertyType.Integer)
            {
                if (property.type == "int")
                {
                    newField = new IntegerField(preferredLabel);
                    ((TextInputBaseField<int>)newField).isDelayed = true;
                }
                else if (property.type == "long")
                {
                    newField = new LongField(preferredLabel);
                    ((TextInputBaseField<long>)newField).isDelayed = true;
                }
            }
            else if (property.propertyType == SerializedPropertyType.String)
            {
                newField = new TextField(preferredLabel);
                ((TextInputBaseField<string>)newField).isDelayed = true;
            }

            if (newField != null)
            {
                newField.bindingPath = property.propertyPath;
                newField.AddToClassList(TextField.alignedFieldUssClassName);
                return newField;
            }

            return new Label(InvalidTypeMessage);
        }
    }
}