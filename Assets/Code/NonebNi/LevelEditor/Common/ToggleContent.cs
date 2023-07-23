using System;
using UnityEngine;

namespace NonebNi.LevelEditor.Common
{
    /// <summary>
    ///     Directly stripped from Unity's ProGrid's ToggleContent
    /// </summary>
    [Serializable]
    internal class ToggleContent
    {
        public Texture2D imageOn, imageOff;
        public string tooltip;
        public readonly string TextOn, TextOff;

        private GUIContent _guiContent = new();

        public ToggleContent(
            string onText,
            string offText,
            Texture2D onImage,
            Texture2D offImage,
            string tip)
        {
            TextOn = onText;
            TextOff = offText;
            imageOn = onImage;
            imageOff = offImage;
            tooltip = tip;

            _guiContent.tooltip = tooltip;
        }

        public static bool ToggleButton(
            Rect r,
            ToggleContent content,
            bool enabled,
            GUIStyle imageStyle,
            GUIStyle altStyle)
        {
            content._guiContent.image = enabled ?
                content.imageOn :
                content.imageOff;
            content._guiContent.text = content._guiContent.image == null ?
                enabled ?
                    content.TextOn :
                    content.TextOff :
                "";

            return GUI.Button(
                r,
                content._guiContent,
                content._guiContent.image != null ?
                    imageStyle :
                    altStyle
            );
        }
    }
}