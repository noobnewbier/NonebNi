using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Logging;
using UnityUtils;

namespace Noneb.Tags.Runtime
{
    internal class CodeTagSource : INonebTagSource
    {
        private readonly Dictionary<NonebTag, string> _tagsAndDescriptions = new ();

        public CodeTagSource(Type type)
        {
            var fields = ReflectionUtils.GetFieldsByAttribute(type, typeof(NonebTagAttribute), bindingFlags: BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var (field, attribute) in fields)
            {
                var value = field.GetValue(null);
                if (value is not string stringValue)
                {
                    Log.Error($"TagSource '{Name}': Field '{field.Name}' has no value");
                    continue;
                }

                if (attribute is not NonebTagAttribute tagAttribute)
                {
                    Log.Error("How are you even getting here - unexpected type {type}", attribute.GetType());
                    continue;
                }

                var tag = new NonebTag(stringValue);
                _tagsAndDescriptions[tag] = tagAttribute.Description;
            }

            Name = type.Name;
        }

        public string Name { get; }

        public IEnumerable<NonebTag> GetTags() => _tagsAndDescriptions.Keys;

        public string FindDescription(NonebTag tag)
        {
            if (!_tagsAndDescriptions.TryGetValue(tag, out var description)) return string.Empty;

            return description;
        }
    }
}