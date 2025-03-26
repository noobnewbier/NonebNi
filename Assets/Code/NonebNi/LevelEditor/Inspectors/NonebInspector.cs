using System;
using NonebNi.LevelEditor.Di;
using NonebNi.Main;
using StrongInject;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace NonebNi.LevelEditor.Inspectors
{
    public class NonebInspector : IDisposable
    {
        private Owned<LevelInspector>? _levelInspector;

        public NonebInspector()
        {
            TryInitLevelInspector();

            NonebEditorUpdatableSystem.Update += OnUpdate;
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void Dispose()
        {
            CleanUp();
        }

        private void OnUpdate()
        {
            if (_levelInspector != null) return;

            TryInitLevelInspector();
        }

        private void OnActiveSceneChanged(Scene _, Scene __)
        {
            TryInitLevelInspector();
        }

        private void TryInitLevelInspector()
        {
            _levelInspector?.Dispose();
            _levelInspector = null;

            var levelRunner = Object.FindObjectOfType<LevelRunner>();
            if (levelRunner == null) return;

            var levelData = levelRunner.LevelData;
            if (levelData == null) return;

            var terrainConfig = levelRunner.TerrainConfig;
            if (terrainConfig == null) return;

            _levelInspector = new RuntimeInspectorContainer(terrainConfig, levelData.Map).Resolve();
        }

        private void CleanUp()
        {
            _levelInspector?.Dispose();
        }

        ~NonebInspector()
        {
            CleanUp();
        }
    }
}