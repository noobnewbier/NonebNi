using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityUtils.Constants;
using UnityUtils.Serialization;

namespace Noneb.Tags.Runtime
{
    [CreateAssetMenu(fileName = nameof(FileTagSource), menuName = MenuName.Data + nameof(FileTagSource))]
    internal class FileTagSource : ScriptableObject, INonebTagSource
    {
        [field: SerializeField] public SerializableDictionary<string, string> TagAndDescriptions { get; private set; } = new ();

        public string Name => name;

        public IEnumerable<NonebTag> GetTags()
        {
            return TagAndDescriptions.Keys.Select(k => new NonebTag(k));
        }

        public string FindDescription(NonebTag tag)
        {
            if (!TagAndDescriptions.TryGetValue(tag, out var description)) return string.Empty;

            return description;
        }
    }
}