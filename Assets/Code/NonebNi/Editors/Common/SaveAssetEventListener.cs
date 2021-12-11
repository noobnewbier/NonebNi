using System.Collections.Generic;

namespace NonebNi.Editors.Common
{
    public class SaveAssetEventListener : UnityEditor.AssetModificationProcessor
    {
        private static string[] OnWillSaveAssets(string[] paths)
        {
            PreSaveAssets?.Invoke(paths);

            return paths;
        }

        private static event OnPreSaveAssets? PreSaveAssets;

        private delegate void OnPreSaveAssets(IReadOnlyList<string> paths);
    }
}