using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Logging;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityUtils;

namespace NonebNi.CustomInspector.AbstractClass
{
    /// <summary>
    /// Generic is not supported atm. We can do it when we need it.
    /// </summary>
    public class TypePickerDropdown : AdvancedDropdown
    {
        private readonly Type _rootType;

        public TypePickerDropdown(Type rootType, AdvancedDropdownState state) : base(state)
        {
            _rootType = rootType;
            minimumSize = new Vector2(200, 200);
        }

        public event Action<Type?>? NewTypeSelected;

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(string.Empty);

            var hierarchy = _rootType.GetTypeHierarchy();
            foreach (var item in hierarchy.Children.Select(CreateItem).SelectMany(childItems => childItems)) root.AddChild(item);

            return root;
        }

        private IEnumerable<AdvancedDropdownItem> CreateItem(TypeHierarchy hierarchy)
        {
            //TODO: finish this so everything is typed and this is actually used
            var type = hierarchy.Value;
            var root = new TypePickerItem(type);

            foreach (var item in hierarchy.Children.Select(CreateItem).SelectMany(childItems => childItems)) root.AddChild(item);

            if (type.IsConcrete())
            {
                yield return root;
                if (hierarchy.Children.Any()) yield return new TypePickerItem($"{type.Name} (AsIs)", type);
            }
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is not TypePickerItem pickerItem)
            {
                var defaultType = _rootType.GetTypeHierarchy().Flatten().FirstOrDefault(t => t.IsConcrete());
                if (defaultType == null) Log.Error("No default type can be found, is it all abstract?");

                NewTypeSelected?.Invoke(defaultType);
                return;
            }

            NewTypeSelected?.Invoke(pickerItem.Type);
        }
    }
}