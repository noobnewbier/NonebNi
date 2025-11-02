using Noneb.Tags.Runtime;
using UnityEditor.IMGUI.Controls;

namespace Noneb.Tags.Editor.TagPickers
{
    public class TagPickerItem : AdvancedDropdownItem
    {
        public readonly NonebTag Tag;

        public TagPickerItem(NonebTag tag) : base(tag.DisplayName)
        {
            Tag = tag;
        }
    }
}