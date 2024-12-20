using System.IO;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.EditorScripting
{
    /// <summary>
    /// Animation from Mixamo have some quirks. Such as names of animation always being mixamo.com.
    /// This little tool fixes it programatically.
    /// Note:
    /// feel free to throw this away future me, I made it to ease testing animation with mixamo but for the most part I
    /// don't think we need it in the long term.
    /// </summary>
    public class MixamoAnimationFixer : EditorWindow
    {
        private string _directory = string.Empty;
        private string TargetFolder => $"{_directory}/output";

        private void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                _directory = EditorGUILayout.TextField("Directory", _directory);
                if (GUILayout.Button("...", GUILayout.ExpandWidth(false)))
                {
                    var selectedFullPath = EditorUtility.OpenFolderPanel("TargetFolder", string.Empty, string.Empty);
                    _directory = selectedFullPath.ToProjectRelativePath();
                }
            }

            var canExtract = AssetDatabase.IsValidFolder(_directory);
            using (new GUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!canExtract))
                {
                    if (GUILayout.Button("Extract", GUILayout.ExpandWidth(false))) ExtractAnimation();
                    if (GUILayout.Button("Rename", GUILayout.ExpandWidth(false))) RenameAnimation();
                }
            }
        }

        [MenuItem("NonebNi/Scripts/Mixamo Animation Fixer")]
        public static void ShowWindow()
        {
            GetWindow(typeof(MixamoAnimationFixer), false, nameof(MixamoAnimationFixer));
        }

        private void RenameAnimation()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                foreach (var clipAsSubAsset in NonebEditorUtils.LoadAllAssetsInFolder<AnimationClip>(_directory))
                {
                    var mainAssetPath = AssetDatabase.GetAssetPath(clipAsSubAsset);
                    if (!AssetDatabase.IsSubAsset(clipAsSubAsset)) continue;

                    var mainAssetName = Path.GetFileName(mainAssetPath);
                    var importer = (ModelImporter)AssetImporter.GetAtPath(mainAssetPath);
                    var importerClips = importer.defaultClipAnimations;
                    foreach (var c in importerClips) c.name = mainAssetName;

                    importer.clipAnimations = importerClips;
                    importer.SaveAndReimport();
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        private void ExtractAnimation()
        {
            using (new NonebEditorUtils.AssetDatabaseEditingScope())
            {
                if (!AssetDatabase.IsValidFolder(TargetFolder)) AssetDatabase.CreateFolder(_directory, "output");

                foreach (var asset in NonebEditorUtils.LoadAllMainAssetsInFolder(_directory))
                {
                    var fbx = AssetDatabase.GetAssetPath(asset);
                    ExtractAnimFromFbx(fbx, $"{_directory}/output");
                }
            }
        }

        private static void ExtractAnimFromFbx(string fbxPath, string target)
        {
            var fileName = Path.GetFileNameWithoutExtension(fbxPath);
            var filePath = $"{target}/{fileName}.anim";
            var src = AssetDatabase.LoadAssetAtPath<AnimationClip>(fbxPath);
            var temp = new AnimationClip();
            EditorUtility.CopySerialized(src, temp);
            AssetDatabase.CreateAsset(temp, filePath);
            AssetDatabase.SaveAssets();
        }
    }
}