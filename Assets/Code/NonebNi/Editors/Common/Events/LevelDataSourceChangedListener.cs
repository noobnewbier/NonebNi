using System;
using System.Linq;
using JetBrains.Annotations;
using NonebNi.Editors.Level.Data;
using UnityEditor;
using UnityEditor.Experimental;
using Object = UnityEngine.Object;

namespace NonebNi.Editors.Common.Events
{
    [UsedImplicitly]
    public class LevelDataSourceChangedListener
    {
        internal static event Action? OnLevelDataSourceChanged;


        [UsedImplicitly]
        private class LevelDataSourceModifiedListener : AssetsModifiedProcessor
        {
            protected override void OnAssetsModified(string[] changedAssets,
                                                     string[] addedAssets,
                                                     string[] deletedAssets,
                                                     AssetMoveInfo[] movedAssets)
            {
                var existingAssets = changedAssets.Concat(addedAssets)
                                                  .Where(p => p.EndsWith(".asset"))
                                                  .Select(AssetDatabase.LoadAssetAtPath<Object>);
                var levelDataSources = existingAssets.OfType<EditorLevelDataSource>();
                var deletedDataSource = deletedAssets.Select(AssetDatabase.LoadAssetAtPath<Object>)
                                                     .OfType<EditorLevelDataSource>();

                if (levelDataSources.Any() || deletedDataSource.Any()) OnLevelDataSourceChanged?.Invoke();
            }
        }

        private class LevelDataSourceDeletedListener : UnityEditor.AssetModificationProcessor
        {
            private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
            {
                //The doc actually explicit says don't call any AssetDatabase APIs within OnWillDeleteAsset, but there are no way we can do this type safely without loading the asset, 
                //plus it seems to be working so /shrug
                var isLevelData = AssetDatabase.LoadAssetAtPath<Object>(assetPath) is EditorLevelDataSource;

                if (isLevelData) OnLevelDataSourceChanged?.Invoke();

                return AssetDeleteResult.DidNotDelete;
            }
        }
    }
}