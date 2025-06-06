﻿using System.IO;
using System.Linq;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Common;
using NonebNi.LevelEditor.Level.Factions;
using NonebNi.LevelEditor.Level.Maps;
using NonebNi.Terrain;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityUtils.Constants;

namespace NonebNi.LevelEditor.Level
{
    /// <summary>
    ///     Created this as we anticipate the need for custom BGMs and stuffs in the future
    /// </summary>
    [CreateAssetMenu(
        fileName = nameof(EditorLevelDataSource),
        menuName = MenuName.Data + nameof(EditorLevelDataSource)
    )]
    public class EditorLevelDataSource : ScriptableObject
    {
        [SerializeField] private LevelDataSource? dataSource;

        /// <summary>
        ///     The scene that should be using this settings, so instead of the scene holding a reference to
        ///     <see cref="EditorLevelDataSource" />,
        ///     the <see cref="EditorLevelDataSource" /> holds a reference to the scene itself,
        ///     and we use it to backtrack and find which level data we should be using.
        /// </summary>
        [SerializeField] private SceneAsset? scene;

        [SerializeField] private EditorMap editorMap = new(10, 10);

        //TODO: this should really be synced with LevelRunner.terrainConfig - I don't know what happened with this mess :), atm these two might not be synced and there's no mechanism to make sure they are either.
        [FormerlySerializedAs("worldConfigScriptable")] [SerializeField]
        private TerrainConfigSource? terrainConfigScriptable;

        //TODO: somehow fit in meshes info here.
        [SerializeField] private string levelName = string.Empty;
        [SerializeField] private EditorFactionsDataSource factionsDataSource = null!;

        public string LevelName => levelName;

        public string? SceneName => scene != null ?
            scene.name :
            null;

        public bool IsValid => terrainConfigScriptable != null && scene != null;

        public TerrainConfigSource? TerrainConfig
        {
            get => terrainConfigScriptable;
            set
            {
                terrainConfigScriptable = value;
                EditorUtility.SetDirty(this);
            }
        }

        public EditorLevelData? CreateData() =>
            IsValid ?
                new EditorLevelData(
                    terrainConfigScriptable!.CreateData(),
                    editorMap,
                    levelName,
                    factionsDataSource.Factions
                ) :
                null;


        public void CopyFromData(EditorLevelData editorLevelData)
        {
            editorMap = editorLevelData.Map;
            if (terrainConfigScriptable != null)
            {
                terrainConfigScriptable.CopyFromData(editorLevelData.TerrainConfig);
            }
            else
            {
                terrainConfigScriptable = TerrainConfigSource.Create(editorLevelData.TerrainConfig);
                AssetDatabase.CreateAsset(terrainConfigScriptable, $"{NonebEditorPaths.GameConfig}Settings.asset");
            }

            if (dataSource == null)
            {
                dataSource = CreateInstance<LevelDataSource>();

                var scenePath = AssetDatabase.GetAssetPath(scene);
                var sceneParentFolder = scenePath.Remove(scenePath.LastIndexOf(Path.AltDirectorySeparatorChar) + 1);

                AssetDatabase.CreateAsset(dataSource, $"{sceneParentFolder}{levelName}GameData.asset");
            }

            dataSource.WriteData(editorLevelData.ToLevelData());
            factionsDataSource.WriteData(editorLevelData.Factions);

            EditorUtility.SetDirty(dataSource);
            EditorUtility.SetDirty(this);
        }

        public static EditorLevelDataSource CreateSource(Scene scene)
        {
            var toReturn = CreateInstance<EditorLevelDataSource>();
            var sceneAssets = AssetDatabase.FindAssets($"t:{nameof(SceneAsset)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<SceneAsset>);
            toReturn.scene = sceneAssets.FirstOrDefault(s => s.name == scene.name);

            var sceneFolder = scene.path.Remove(scene.path.LastIndexOf(Path.AltDirectorySeparatorChar) + 1);
            AssetDatabase.CreateAsset(toReturn, $"{sceneFolder}{scene.name}Settings.asset");

            return toReturn;
        }

        public static EditorLevelDataSource? FindLevelDataSourceForActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            var allLevelDatas = AssetDatabase.FindAssets($"t:{nameof(EditorLevelDataSource)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<EditorLevelDataSource>);
            var matchingData = allLevelDatas.FirstOrDefault(s => s.SceneName == activeScene.name);

            return matchingData;
        }
    }
}