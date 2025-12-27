using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Noneb.Tags.Runtime
{
    [Serializable, DebuggerDisplay("{Name,nq}")]
    public record NonebTag
    {
        public static readonly NonebTag None = new (string.Empty);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] [field: SerializeField] public string Name { get; private set; }

        internal NonebTag(string name)
        {
            Name = name;
        }

        public bool IsNone => string.IsNullOrWhiteSpace(Name);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public IEnumerable<NonebTag> ParentTags
        {
            get
            {
                var split = Name.Split('.');
                if (split.Length <= 1) yield break;

                for (var i = 0; i < split.Length - 1; i++) yield return new (string.Join(".", split.Take(i + 1)));
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)] public int HierarchyLevel => Name.Count(c => c == '.');


        public NonebTag ParentTag => ParentTags.LastOrDefault() ?? None;


        public string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return "<NONE>";

                return Name;
            }
        }

        public bool IsParentOf(in NonebTag tag) => tag.ParentTags.Contains(this);

        public bool IsChildOf(in NonebTag parentTag) => ParentTags.Contains(parentTag);

        public override string ToString() => DisplayName;

        public static implicit operator NonebTag(string tagName) => new (tagName);

        public static implicit operator string(NonebTag tag) => tag.Name;

        public static bool IsNameValid(string name, out string errorMessage)
        {
            static bool IsValidLabelCharacter(char c)
            {
                return char.IsLetterOrDigit(c) || c == '_' || c == '-';
            }

            static bool AcceptLabel(string name, ref int position)
            {
                if (position >= name.Length || !IsValidLabelCharacter(name[position]))
                    return false;

                position++;
                while (position < name.Length && IsValidLabelCharacter(name[position])) position++;

                return true;
            }

            if (string.IsNullOrEmpty(name))
            {
                errorMessage = "Tag name cannot be null or empty.";
                return false;
            }

            var position = 0;
            if (AcceptLabel(name, ref position))
                while (position < name.Length && name[position] == '.')
                {
                    position++;
                    if (!AcceptLabel(name, ref position))
                    {
                        errorMessage = $"Invalid tag name '{name}'. Unexpected character at position {position}.";
                        return false;
                    }
                }

            if (position == name.Length)
            {
                errorMessage = string.Empty;
                return true;
            }

            errorMessage = $"Invalid tag name '{name}'. Unexpected character at position {position}.";
            return false;
        }
    }
}