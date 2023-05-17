using NonebNi.Core.Level;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.EditorComponent.Entities;
using NonebNi.LevelEditor.Level.Data;
using UnityEditor;
using UnityEngine;
using UnityUtils.Editor;
using UnityUtils.Factories;

namespace NonebNi.LevelEditor.Level.Inspector
{
    public class TileInspectorView
    {
        private const float MinRectWidth = 100;
        private static readonly Vector2 TitleSize = new(MinRectWidth, 20);
        private static readonly Vector2 ContentSize = new(MinRectWidth, 100);
        public static readonly Vector2 WindowSize = new(MinRectWidth, TitleSize.y + ContentSize.y);

        private static readonly int WindowID = nameof(TileInspectorView).GetHashCode();

        private readonly IEditorMap _map;

        private readonly TileInspectorPresenter _presenter;


        private Plane _gridPlane;

        public TileInspectorView(IFactory<TileInspectorView, TileInspectorPresenter> presenterFactory,
            WorldConfigData worldConfigData,
            IEditorMap map)
        {
            _map = map;
            _presenter = presenterFactory.Create(this);

            _gridPlane = new Plane(Vector3.up, worldConfigData.MapStartingPosition);
        }

        public void OnSceneDraw(Vector2 startingPosition)
        {
            if (!_presenter.IsDrawing) return;
            Handles.BeginGUI();

            var rect = new Rect(startingPosition, WindowSize);
            GUI.Box(rect, GUIContent.none, NonebGUIStyle.SceneHelpWindow);

            var titleRect = new Rect(startingPosition, TitleSize);
            GUI.Label(titleRect, "TileInspector", NonebGUIStyle.Title);

            var contentRect = new Rect(startingPosition + Vector2.up * TitleSize.y, ContentSize);
            using (new GUILayout.AreaScope(contentRect))
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

                if (_map.TryGet<EditorEntityData<TileModifierData>>(coord, out var tileModifier))
                    GUILayout.Label($"TileModifier:\n{tileModifier.ToEntityData().Name}");

                if (_map.TryGet<EditorEntityData<UnitData>>(coord, out var unit))
                    GUILayout.Label($"Unit: {unit.ToEntityData().Name}");
            }


            //Required to force a refresh next frame. Otherwise the inspector won't update as the mouse move.
            EditorApplication.delayCall += SceneView.RepaintAll;

            Handles.EndGUI();
        }
    }
}