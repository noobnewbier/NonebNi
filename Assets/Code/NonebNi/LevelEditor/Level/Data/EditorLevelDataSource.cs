using System.IO;
using System.Linq;
using NonebNi.Core.Level;
using NonebNi.LevelEditor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityUtils.Constants;

namespace NonebNi.LevelEditor.Level.Data
{
    /// <summary>
    /// Created this as we anticipate the need for custom BGMs and stuffs in the future
    /// </summary>
    [CreateAssetMenu(fileName = nameof(EditorLevelDataSource), menuName = MenuName.Data + nameof(EditorLevelDataSource))]
    public class EditorLevelDataSource : ScriptableObject
    {
        [SerializeField] private LevelDataSource? dataSource;

        /// <summary>
        /// The scene that should be using this settings, so instead of the scene holding a reference to
        /// <see cref="EditorLevelDataSource" />,
        /// the <see cref="EditorLevelDataSource" /> holds a reference to the scene itself, and we use it to backtrack and find which
        /// level
        /// data we should be using.
        /// </summary>
        [SerializeField] private SceneAsset? scene;

        [SerializeField] private EditorMap editorMap = new EditorMap(10, 10);
        [SerializeField] private WorldConfigSource? worldConfigScriptable;
        [SerializeField] private string levelName = string.Empty;

        public string LevelName => levelName;
        public string? SceneName => scene != null ? scene.name : null;
        public bool IsValid => worldConfigScriptable != null && scene != null;

        public WorldConfigSource? WorldConfig
        {
            get => worldConfigScriptable;
            set
            {
                worldConfigScriptable = value;
                EditorUtility.SetDirty(this);
            }
        }

        public EditorLevelData? CreateData() =>
            IsValid ? new EditorLevelData(worldConfigScriptable!.CreateData(), editorMap, levelName) : null;

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

        public void CopyFromData(EditorLevelData editorLevelData)
        {
            editorMap = editorLevelData.Map;
            if (worldConfigScriptable != null)
            {
                worldConfigScriptable.CopyFromData(editorLevelData.WorldConfig);
            }
            else
            {
                worldConfigScriptable = WorldConfigSource.Create(editorLevelData.WorldConfig);
                AssetDatabase.CreateAsset(worldConfigScriptable, $"{NonebEditorPaths.GameConfig}Settings.asset");
            }

            if (dataSource == null)
            {
                dataSource = CreateInstance<LevelDataSource>();

                var scenePath = AssetDatabase.GetAssetPath(scene);
                var sceneParentFolder = scenePath.Remove(scenePath.LastIndexOf(Path.AltDirectorySeparatorChar) + 1);

                AssetDatabase.CreateAsset(dataSource, $"{sceneParentFolder}{levelName}GameData.asset");
            }

            dataSource.WriteData(editorLevelData.ToLevelData());

            EditorUtility.SetDirty(dataSource);
            EditorUtility.SetDirty(this);
        }
    }
}