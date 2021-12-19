using System.Diagnostics;
using System.IO;
using System.Linq;
using NonebNi.Editors.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityUtils.Constants;

namespace NonebNi.Editors.Level.Data
{
    /// <summary>
    /// Created this as we anticipate the need for custom BGMs and stuffs in the future
    /// </summary>
    [CreateAssetMenu(fileName = nameof(LevelDataSource), menuName = MenuName.Data + nameof(LevelDataSource))]
    public class LevelDataSource : ScriptableObject
    {
        /// <summary>
        /// The scene that should be using this settings, so instead of the scene holding a reference to <see cref="LevelDataSource" />,
        /// the <see cref="LevelDataSource" /> holds a reference to the scene itself, and we use it to backtrack and find which level
        /// data we should be using.
        /// </summary>
        [SerializeField] private SceneAsset? scene;

        [FormerlySerializedAs("map")] [SerializeField]
        private EditorMap editorMap = new EditorMap(10, 10);

        [SerializeField] private WorldConfigSource? worldConfigScriptable;

        public string? SceneName => scene != null ? scene.name : null;

        public bool IsValid => worldConfigScriptable != null && scene != null;

        public WorldConfigSource? WorldConfig
        {
            get => worldConfigScriptable;
            set
            {
                worldConfigScriptable = value;
                DirtyThis();
            }
        }

        [Conditional("UNITY_EDITOR")]
        private void DirtyThis()
        {
            EditorUtility.SetDirty(this);
        }

        public EditorLevelData? CreateData() =>
            IsValid ? new EditorLevelData(worldConfigScriptable!.CreateData(), editorMap) : null;

        public static LevelDataSource CreateSource(Scene scene)
        {
            var toReturn = CreateInstance<LevelDataSource>();
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

            DirtyThis();
        }
    }
}