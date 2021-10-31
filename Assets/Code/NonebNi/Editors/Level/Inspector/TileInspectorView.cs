﻿using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Editors.Di;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;

namespace NonebNi.Editors.Level.Inspector
{
    public class TileInspectorView
    {
        private const float MinRectHeight = 100;
        private const float MinRectWidth = 100;
        public static readonly Vector2 WindowSize = new Vector2(MinRectWidth, MinRectHeight);

        private static readonly int WindowID = nameof(TileInspectorView).GetHashCode();

        private readonly IReadOnlyMap _map;

        private readonly TileInspectorPresenter _presenter;


        private Plane _gridPlane;

        public TileInspectorView(ILevelEditorComponent component, WorldConfigData worldConfigData, IReadOnlyMap map)
        {
            _map = map;
            _presenter = component.CreateTileInspectorPresenter(this);

            _gridPlane = new Plane(Vector3.up, worldConfigData.MapStartingPosition);
        }

        public void OnSceneDraw(Vector2 position)
        {
            if (!_presenter.IsDrawing) return;

            var rect = new Rect(position, WindowSize);

            void WindowFunc(int _)
            {
                void DrawNotInspectingWindow()
                {
                    GUILayout.Label("Not inspecting.", NonebGUIStyle.Hint);
                }

                if (!Camera.current)
                {
                    DrawNotInspectingWindow();
                    return;
                }

                if (SceneView.lastActiveSceneView != EditorWindow.mouseOverWindow)
                {
                    DrawNotInspectingWindow();
                    return;
                }

                var ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                if (!_gridPlane.Raycast(ray, out var distance))
                {
                    DrawNotInspectingWindow();
                    return;
                }

                var coord = _presenter.FindCoordinate(ray.GetPoint(distance));
                if (!_map.IsCoordinateWithinMap(coord))
                {
                    DrawNotInspectingWindow();
                    return;
                }

                GUILayout.Label(coord.ToString());

                if (_map.TryGet(coord, out var tile)) GUILayout.Label($"Weight: {tile.Weight}");
                else GUILayout.Label("TILE IS NOT VALID", NonebGUIStyle.Error);
            }

            GUI.Window(
                WindowID,
                rect,
                WindowFunc,
                "TileInspector"
            );

            //Required to force a refresh next frame. Otherwise the inspector won't update as the mouse move.
            EditorApplication.delayCall += SceneView.RepaintAll;
        }
    }
}