﻿using NonebNi.LevelEditor.Common;
using UnityEditor;
using UnityEngine;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Toolbar
{
    //todo: allow toggle of toolbar itself
    public class NonebEditorToolbarView
    {
        private readonly NonebEditorToolbarPresenter _presenter;

        public NonebEditorToolbarView(IFactory<NonebEditorToolbarView, NonebEditorToolbarPresenter> presenterFactory)
        {
            _presenter = presenterFactory.Create(this);
        }

        public void DrawSceneToolbar(SceneView sceneView)
        {
            //reusing the same rect through out. offsetting the y position after drawing of each button
            var menuRect = Styles.StartingButtonRect;

            void AddSpacing()
            {
                menuRect.y += Styles.MenuItemPadding + menuRect.height;
            }

            Handles.BeginGUI();
            var srgb = GL.sRGBWrite;
            GL.sRGBWrite = false;

            DrawSettingsButton();

            AddSpacing();

            DrawGridButton();

            AddSpacing();

            DrawGizmosButton();

            GUI.backgroundColor = Color.white;
            GL.sRGBWrite = srgb;
            Handles.EndGUI();

            void DrawGridButton()
            {
                if (ToggleContent.ToggleButton(
                        menuRect,
                        Contents.GridEnabled,
                        _presenter.IsGridVisible,
                        Styles.ToggleButton,
                        EditorStyles.miniButton
                    )) _presenter.OnToggleGridVisibility();
            }

            void DrawGizmosButton()
            {
                if (ToggleContent.ToggleButton(
                        menuRect,
                        Contents.GizmosEnabled,
                        _presenter.IsHelperWindowsVisible,
                        Styles.ToggleButton,
                        EditorStyles.miniButton
                    )) _presenter.OnToggleHelperWindowsVisibility();
            }

            void DrawSettingsButton()
            {
                if (_presenter.TryGetSettingsWindow(out var settingsWindow))
                {
                    /*
                     * There is a minor issue where if the user have already opened the window
                     * Clicking the button again will close the window and open the window again in quick fashion, leading to quick flickers.
                     *
                     * Root of the problem is that whenever you click on space that is outside of the window, the window closes immediately,
                     * and this button quickly re-opens the closed window.
                     * 
                     * This happens with ProGrids as well,
                     * I suppose I can do some clever trick, such as recording the visibility of the editor on the past few frames,
                     * or simply preventing the window to be re-opened when it is closed this frame,
                     * But those are a bit hacky and dependent on Unity, plus this is not that big of an issue so.
                     */
                    if (GUI.Button(menuRect, "Settings", Styles.Button))
                    {
                        var screenRect = sceneView.position;
                        settingsWindow.ShowAsDropDown(
                            new Rect(
                                screenRect.x + menuRect.x + menuRect.width + Styles.MenuItemPadding,
                                screenRect.y + Styles.MenuStartingPosition.y + 24 + Styles.MenuItemPadding,
                                0,
                                0
                            ),
                            new Vector2(450, 140)
                        );
                    }
                }
                else
                {
                    if (GUI.Button(menuRect, "InitLevel", Styles.Button)) _presenter.ConvertActiveSceneToLevel();
                }
            }
        }

        private static class Contents
        {
            public static readonly ToggleContent GizmosEnabled = new(
                "Hide",
                "Show",
                IconUtility.LoadIcon("NonebNi_GUI_Gizmos_On.png"),
                IconUtility.LoadIcon("NonebNi_GUI_Gizmos_Off.png"),
                "Toggles drawing of gizmos."
            );

            public static readonly ToggleContent GridEnabled = new(
                "Hide",
                "Show",
                IconUtility.LoadIcon("NonebNi_GUI_Grid_On.png"),
                IconUtility.LoadIcon("NonebNi_GUI_Grid_Off.png"),
                "Toggles drawing of the map grid."
            );
        }

        private static class Styles
        {
            public const int MenuItemPadding = 3;
            public static readonly GUIStyle ToggleButton = new();

            public static readonly GUIStyle Button = new()
            {
                fontSize = 8,
                alignment = TextAnchor.MiddleCenter,
                normal = new GUIStyleState
                {
                    background = IconUtility.LoadIcon("NonebNi_GUI_LevelSettings.png"),
                    textColor = new Color(0.75f, 0.75f, 0.75f)
                },
                hover = new GUIStyleState
                {
                    background = IconUtility.LoadIcon("NonebNi_GUI_LevelSettings.png"),
                    textColor = new Color(0.75f, 0.75f, 0.75f)
                }
            };

            public static readonly Vector2Int MenuStartingPosition = new(8, 8 + MenuItemPadding);

            public static readonly Rect StartingButtonRect = new(
                MenuStartingPosition.x,
                MenuStartingPosition.y,
                42,
                16
            );
        }
    }
}