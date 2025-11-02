using System;
using System.Collections.Generic;
using System.Linq;
using Noneb.Tags.Runtime;
using UnityEditor.IMGUI.Controls;

namespace Noneb.Tags.Editor.TagPickers
{
    public class TagPickerDropdown : AdvancedDropdown
    {
        public TagPickerDropdown() : base(new ())
        {
            minimumSize = new (200, 200);
        }

        public event Action<NonebTag>? NewTagSelected;

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem(string.Empty);

            foreach (var item in CreateRootItems()) root.AddChild(item);

            return root;
        }

        private IEnumerable<AdvancedDropdownItem> CreateRootItems()
        {
            /*
             * Note:
             * AdvancedDropdownItem wouldn't let you remove children, so either you add items in a sorted order,
             * or you have to have a cache of items that you want to add, sort them, and then add the sorted list.
             *
             * I did think of going the reflection route - it felt overkill so hey here we go.
             */
            var itemAndUncommitedChildren = EditorNonebTagManager.GetAllTags()
                .Select(t => new TagPickerItem(t))
                .ToDictionary(tag => tag, _ => new List<TagPickerItem>());

            // add child relationship for the foldout like structure
            foreach (var a in itemAndUncommitedChildren.Keys)
            foreach (var b in itemAndUncommitedChildren.Keys.Where(b => EditorNonebTagManager.IsDirectParent(a.Tag, b.Tag)))
                itemAndUncommitedChildren[a].Add(b);

            // if you have any children I can't actually select you, so make an empty item just for ya.
            foreach (var item in itemAndUncommitedChildren.Keys.ToArray())
                if (itemAndUncommitedChildren[item].Any())
                {
                    var optionForSelf = new TagPickerItem(item.Tag);
                    itemAndUncommitedChildren[item].Add(optionForSelf);

                    itemAndUncommitedChildren[optionForSelf] = new ();
                }

            // sort those children into something human friendly
            foreach (var (item, children) in itemAndUncommitedChildren)
                children.Sort((a, b) =>
                    {
                        // folder goes in first
                        if (itemAndUncommitedChildren[a].Any() && !itemAndUncommitedChildren[b].Any()) return -1;

                        if (itemAndUncommitedChildren[b].Any() && !itemAndUncommitedChildren[a].Any()) return 1;

                        // self goes second, ignoring name.
                        if (a.Tag == item.Tag) return -1;

                        // rest of the lads go alphabetically
                        return string.Compare(a.Tag.Name, b.Tag.Name, StringComparison.Ordinal);
                    }
                );

            // Commit those list of children
            foreach (var (tag, children) in itemAndUncommitedChildren)
            foreach (var child in children)
                tag.AddChild(child);

            // return the root item
            var rootItems = itemAndUncommitedChildren.Keys.Where(tagItem => tagItem.Tag.HierarchyLevel == 0).ToArray();
            var rootItemsWithChild = rootItems.Where(i => i.children.Any()).ToArray();
            var rootItemsWithNoChild = rootItems.Except(rootItemsWithChild).Where(i => !rootItemsWithChild.Any(n => i != n && i.Tag == n.Tag)).ToArray();

            foreach (var item in rootItemsWithChild) yield return item;

            foreach (var item in rootItemsWithNoChild) yield return item;

            // and append a none on the end
            yield return new TagPickerItem(NonebTag.None);
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            base.ItemSelected(item);
            if (item is not TagPickerItem pickerItem)
            {
                var defaultTag = NonebTag.None;
                NewTagSelected?.Invoke(defaultTag);

                return;
            }

            NewTagSelected?.Invoke(pickerItem.Tag);
        }
    }
}