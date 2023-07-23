using System.Collections.Generic;
using UnityEditor;

namespace NonebNi.LevelEditor.Common.Events
{
    public class SaveAssetEventListener : AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            OnPreSaveAssetsEvent?.Invoke(paths);

            return paths;
        }

        internal static event OnWillSaveAssetsDelegate? OnPreSaveAssetsEvent;

        internal delegate void OnWillSaveAssetsDelegate(IReadOnlyList<string> paths);
    }
}