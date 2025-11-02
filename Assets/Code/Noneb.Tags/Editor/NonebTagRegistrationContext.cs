using System.Collections.Generic;
using System.Linq;
using Noneb.Tags.Runtime;
using Unity.Logging;
using UnityUtils;
using UnityUtils.Editor;

namespace Noneb.Tags.Editor
{
    /// <summary>
    /// Note: if the day comes where we need a runtime tag manager, we can feed in the context
    /// </summary>
    internal class NonebTagRegistrationContext
    {
        private readonly Dictionary<INonebTagSource, HashSet<NonebTag>> _tags = new ();

        public NonebTagRegistrationContext()
        {
            // Register tags from all assemblies with the GameplayTagAttribute attribute.
            foreach (var (type, _) in ReflectionUtils.GetTypesWithAttribute<NonebTagAttribute>())
            {
                var source = new CodeTagSource(type);
                var tags = source.GetTags();
                foreach (var tag in tags) RegisterTag(tag, source);
            }

            /*
             * Note:
             * If the day comes where we need a runtime tag manager, we can feed in the file tag source instead.
             * For now, this is good enough.
             */
            foreach (var source in NonebEditorGUI.FindAssetsByType<FileTagSource>())
            {
                var tags = source.GetTags();
                foreach (var tag in tags) RegisterTag(tag, source);
            }
        }

        private void RegisterTag(NonebTag tag, INonebTagSource source)
        {
            if (!NonebTag.IsNameValid(tag, out var errorMessage))
            {
                Log.Error($"Failed to register gameplay tag \"{tag}\": {errorMessage} (Source: {source.Name})");
                return;
            }

            // Tags can duplicate in other sources
            if (!_tags.TryGetValue(source, out var set)) _tags[source] = set = new ();
            if (!set.Add(tag)) return;

            // work with the parent tags as well.
            var parentTags = tag.ParentTags;
            foreach (var parent in parentTags) RegisterTag(parent, source);
        }

        public NonebTag[] GetAllTags() => _tags.Values.SelectMany(s => s).ToArray();

        public IEnumerable<INonebTagSource> FindSources(NonebTag tag) => _tags.Keys.Where(s => _tags[s].Contains(tag));
    }
}