using NonebNi.Editor.Common;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps.Toolbar
{
    //todo: allow toggle of toolbar itself
    public class MapEditorSceneToolbarView
    {
        private static class Contents
        {
            public static readonly ToggleContent GizmosEnabled = new ToggleContent(
                "Hide",
                "Show",
                IconUtility.LoadIcon("NonebNi_GUI_Gizmos_On.png"),
                IconUtility.LoadIcon("NonebNi_GUI_Gizmos_Off.png"),
                "Toggles drawing of gizmos."
            );

            public static readonly ToggleContent GridEnabled = new ToggleContent(
                "Hide",
                "Show",
                IconUtility.LoadIcon("NonebNi_GUI_Grid_On.png"),
                IconUtility.LoadIcon("NonebNi_GUI_Grid_Off.png"),
                "Toggles drawing of the map grid."
            );
        }

        private static class Styles
        {
            public static readonly GUIStyle Button = new GUIStyle();

            public const int MenuItemPadding = 3;

            private static readonly Vector2Int MenuStartingPosition = new Vector2Int(8, 8 + MenuItemPadding);

            public static readonly Rect StartingButtonRect = new Rect(
                MenuStartingPosition.x,
                MenuStartingPosition.y,
                42,
                16
            );
        }

        private readonly MapEditorSceneToolbarPresenter _presenter;

        public MapEditorSceneToolbarView()
        {
            _presenter = new MapEditorSceneToolbarPresenter(this);
        }

        public void DrawSceneToolbar()
        {
            Handles.BeginGUI();

            var srgb = GL.sRGBWrite;
            GL.sRGBWrite = false;

            //reusing the same rect through out. offsetting the y position after drawing of each button
            var buttonRect = Styles.StartingButtonRect;

            // Draw grid
            if (ToggleContent.ToggleButton(
                buttonRect,
                Contents.GridEnabled,
                MapEditor.Instance?.IsDrawMapOverlay ?? false,
                Styles.Button,
                EditorStyles.miniButton
            ))
            {
                _presenter.OnToggleGridVisibility();
            }

            buttonRect.y += Styles.MenuItemPadding + buttonRect.height;

            if (ToggleContent.ToggleButton(
                buttonRect,
                Contents.GizmosEnabled,
                MapEditor.Instance?.IsDrawGizmosOverlay ?? false,
                Styles.Button,
                EditorStyles.miniButton
            ))
            {
                _presenter.OnToggleGizmosVisibility();
            }

            GUI.backgroundColor = Color.white;
            GL.sRGBWrite = srgb;
            
            Handles.EndGUI();
        }
    }
}