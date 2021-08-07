using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Common
{
    internal static class IconUtility
    {
        private const string IconPath = "Assets/EditorAssets/GUI/Icons/";

        public static Texture2D LoadIcon(string iconName)
        {
            var iconPath = IconPath + iconName;
            var img = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);

            if (!img)
                Debug.LogError($"Editor failed to locate menu image: {iconName}");

            return img;
        }
    }
}