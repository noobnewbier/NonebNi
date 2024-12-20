using System;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace NonebNi.CustomInspector.AttributeDrawers
{
    /// <summary>
    /// In the future we might want to make a more generalized version of this, for now though this suffice
    /// the way we need to use callback is quite gross though ngl.
    /// </summary>
    public class AnyTypeAnimatorParameterDropdown : AdvancedDropdown
    {
        private readonly RuntimeAnimatorController _animatorController;

        public AnyTypeAnimatorParameterDropdown(RuntimeAnimatorController animatorController, AdvancedDropdownState state) : base(state)
        {
            _animatorController = animatorController;
            minimumSize = new Vector2(200, 200);
        }

        public event Action<(string paramName, AnimatorControllerParameterType type)?>? NewParamSelected;

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(string.Empty);

            var floats = CreateParameterTypeItems(AnimatorControllerParameterType.Float);
            var ints = CreateParameterTypeItems(AnimatorControllerParameterType.Int);
            var bools = CreateParameterTypeItems(AnimatorControllerParameterType.Bool);
            var triggers = CreateParameterTypeItems(AnimatorControllerParameterType.Trigger);

            if (floats.children.Any()) root.AddChild(floats);
            if (ints.children.Any()) root.AddChild(ints);
            if (bools.children.Any()) root.AddChild(bools);
            if (triggers.children.Any()) root.AddChild(triggers);

            root.AddChild(new AdvancedDropdownItem("Remove Selection"));

            return root;
        }

        private AdvancedDropdownItem CreateParameterTypeItems(AnimatorControllerParameterType type)
        {
            //TODO: finish this so everything is typed and this is actually used
            var paramTable = AnimatorInfoCache.GetParamTable(_animatorController);
            var parameters = paramTable.GetParameters(type);

            var typeItem = new AdvancedDropdownItem(type.ToString());
            foreach (var param in parameters)
            {
                var item = new AnimatorParameterPickerItem(param, type);
                typeItem.AddChild(item);
            }

            return typeItem;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is not AnimatorParameterPickerItem pickerItem)
            {
                NewParamSelected?.Invoke(null);
                return;
            }

            //TODO: try not subscribe should thr?
            NewParamSelected?.Invoke((pickerItem.ParameterName, pickerItem.Type));
        }
    }
}