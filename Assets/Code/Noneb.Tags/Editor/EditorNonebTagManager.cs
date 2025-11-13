using System.Collections.Generic;
using System.Linq;
using Noneb.Tags.Runtime;
using UnityEditor;
using UnityEngine;

namespace Noneb.Tags.Editor
{
    internal static class EditorNonebTagManager
    {
        private static NonebTagRegistrationContext _context = new ();


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            _context = new ();
        }


        public static IEnumerable<INonebTagSource> FindAllSources(NonebTag tag) => _context.FindSources(tag);

        public static string FindDescription(NonebTag tag)
        {
            var sources = FindAllSources(tag);
            var targetSource = sources.FirstOrDefault();
            if (targetSource == null) return "NO DESCRIPTION - NO ASSOCIATED SOURCE";

            var description = targetSource.FindDescription(tag);
            if (string.IsNullOrEmpty(description)) return "NO DESCRIPTION DEFINED";

            return description;
        }

        public static IEnumerable<NonebTag> GetAllTags() => _context.GetAllTags().Append(NonebTag.None);

        public static bool IsDirectParent(NonebTag parent, NonebTag child) => parent.IsParentOf(child) && child.HierarchyLevel - parent.HierarchyLevel == 1;

        private class ReloadOnFileSourceChanges : AssetPostprocessor
        {
            private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
            {
                foreach (var assetPath in importedAssets)
                    if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(FileTagSource))
                    {
                        Init();
                        break;
                    }
            }
        }
    }
}