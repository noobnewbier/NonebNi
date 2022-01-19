using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Editors.Common.Events;
using NonebNi.Editors.Level.Data;
using NonebNi.Editors.Level.Maps;
using NonebNi.Editors.Level.Settings;
using UnityEngine.SceneManagement;

namespace NonebNi.Editors.Level
{
    /// <summary>
    /// Synchronizing the currently edited level(scene) and the backing data(<see cref="EditorLevelDataSource" />).
    /// The data needs to be synced:
    /// 1. Before exiting edit mode(i.e, before entering play mode)
    /// 2. On the "level" scene being saved
    /// </summary>
    public class LevelDataSyncer : IDisposable
    {
        private readonly LevelSavingService _levelSavingService;
        private readonly Scene _levelScene;
        private readonly MapSyncService _mapSyncService;

        public LevelDataSyncer(LevelSavingService levelSavingService, Scene levelScene, MapSyncService mapSyncService)
        {
            _levelSavingService = levelSavingService;
            _levelScene = levelScene;
            _mapSyncService = mapSyncService;

            PlayModeStateListener.OnExitEditMode += Sync;
            SaveAssetEventListener.OnPreSaveAssetsEvent += SyncIfLevelSceneIsSaved;
        }

        public void Dispose()
        {
            PlayModeStateListener.OnExitEditMode -= Sync;
            SaveAssetEventListener.OnPreSaveAssetsEvent -= SyncIfLevelSceneIsSaved;
        }

        private void Sync()
        {
            _mapSyncService.Sync();
            _levelSavingService.Save();
        }

        private void SyncIfLevelSceneIsSaved(IReadOnlyList<string> paths)
        {
            var levelScenePath = _levelScene.path;

            var isLevelSceneSaved = paths.Any(p => p == levelScenePath);
            if (isLevelSceneSaved) Sync();
        }
    }
}