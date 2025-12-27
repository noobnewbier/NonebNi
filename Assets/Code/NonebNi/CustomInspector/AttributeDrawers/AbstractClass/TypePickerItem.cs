using System;
using UnityEditor.IMGUI.Controls;

namespace NonebNi.CustomInspector.AbstractClass
{
    public class TypePickerItem : AdvancedDropdownItem
    {
        public readonly Type Type;

        public TypePickerItem(string name, Type type) : base(name)
        {
            Type = type;
        }

        public TypePickerItem(Type type) : base(type.Name)
        {
            Type = type;
        }

        public bool IsConcrete => Type is { IsAbstract: false, IsGenericTypeDefinition: false };
    }
}