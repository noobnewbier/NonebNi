using System.Linq;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using NonebNi.Editor.Di;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Maps
{
    public class MapView
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;

        private readonly Map _map;
        private readonly MapPresenter _presenter;
        private readonly WorldConfigData _worldConfig;

        public bool IsDrawingGrid { private get; set; }
        public bool IsDrawingGizmos { private get; set; }

        public MapView(ILevelEditorComponent component)
        {
            _presenter = new MapPresenter(this, component);
            _map = component.LevelEditorDataModel.LevelData.Map;
            _worldConfig = component.LevelEditorDataModel.LevelData.WorldConfig;
            _coordinateAndPositionService = component.CoordinateAndPositionService;
        }

        public void OnSceneDraw()
        {
            DrawGrid();
            DrawGizmos();
        }

        private void DrawGrid()
        {
            if (!IsDrawingGrid) return;

            var coordinates = _map.GetAllCoordinates();
            foreach (var (coordinate, tile) in coordinates.Select(c => (c, _map.Get<TileData>(c))))
            {
                if (tile == null) continue;

                var center = _coordinateAndPositionService.FindPosition(coordinate);

                var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
            }
        }

        private void DrawGizmos()
        {
            if (!IsDrawingGizmos) return;

            var coordinates = _map.GetAllCoordinates();
            foreach (var (coordinate, tile, unit) in coordinates.Select(
                c => (c, _map.Get<TileData>(c), _map.Get<UnitData>(c))
            ))
            {
                if (tile == null) continue;

                var centerPosition = _coordinateAndPositionService.FindPosition(coordinate);

                var text = $"{coordinate}";
                if (unit != null) text += $"\n Unit: {unit.Name}";
                DrawCenteredLabel(centerPosition, text, _worldConfig.InnerRadius);
            }
        }

        /// <summary>
        /// <see cref="Handles.Label(UnityEngine.Vector3,string)" /> can't center the label properly.
        /// Root of the problem is that <see cref="HandleUtility.WorldPointToSizedRect" /> isn't taking rects maxX/Y into account,
        /// This leads to a problem where the rect is essentially stretched to the left a bit, and as a result when trying to center the
        /// item,
        /// the text will goes a bit "righter" than it should be.
        /// Until Unity(current ver: 2020.2.7f1) fix this, this should do the trick
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <param name="maxOffsetFromCenter"></param>
        /// <returns></returns>
        private void DrawCenteredLabel(Vector3 position, string text, float maxOffsetFromCenter)
        {
            //behind the camera
            if (HandleUtility.WorldToGUIPointWithDepth(position).z < 0.0)
                return;

            var guiPoint = HandleUtility.WorldToGUIPoint(position);

            //stop drawing the label if the text will be so small that it's invisible
            //define font size in a way it is more less around the same portion of the bounding rect
            const float fontSizeToBoundingRectRatio = 0.125f;
            var viewCameraTransform = SceneView.currentDrawingSceneView.camera.transform;
            var cameraForward = viewCameraTransform.forward;
            var viewForward = new Vector3(cameraForward.x, 0f, cameraForward.z).normalized;
            var viewRotation = Quaternion.LookRotation(viewForward, Vector3.up);

            var v1 = HandleUtility.WorldToGUIPoint(position + viewRotation * Vector3.left * maxOffsetFromCenter);
            var v2 = HandleUtility.WorldToGUIPoint(position + viewRotation * Vector3.right * maxOffsetFromCenter);
            var v3 = HandleUtility.WorldToGUIPoint(position + viewRotation * Vector3.forward * maxOffsetFromCenter);
            var v4 = HandleUtility.WorldToGUIPoint(position + viewRotation * Vector3.back * maxOffsetFromCenter);
            //Which vertices is which corner of the rect depends on the orientation of the camera
            var minX = Mathf.Min(
                v1.x,
                v2.x,
                v3.x,
                v4.x
            );
            var minY = Mathf.Min(
                v1.y,
                v2.y,
                v3.y,
                v4.y
            );
            var maxX = Mathf.Max(
                v1.x,
                v2.x,
                v3.x,
                v4.x
            );
            var maxY = Mathf.Max(
                v1.y,
                v2.y,
                v3.y,
                v4.y
            );
            var boundingRect = new Rect(
                minX,
                minY,
                maxX - minX,
                maxY - minY
            );


            const int maxFontSize = 14;
            var fontSizeInFloat = boundingRect.height * fontSizeToBoundingRectRatio;
            var coordinateStyle = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = Mathf.Min(Mathf.RoundToInt(fontSizeInFloat), maxFontSize)
            };

            var textContent = EditorGUIUtility.TrTempContent(text);
            var size = coordinateStyle.CalcSize(textContent);
            var rect = new Rect(guiPoint, size);
            rect.xMin -= size.x / 2;
            rect.xMax -= size.x / 2;
            rect.yMin -= size.y / 2;
            rect.yMax -= size.y / 2;

            //These are just a magic number that feels right to me
            const int minReadableFontSize = 6;
            const float minVisibleAlpha = 0.5f;
            //decreasing alpha when the user is further away from the text while avoiding drawing text that are practically not readable
            coordinateStyle.normal.textColor = new Color(
                0f,
                0f,
                0f,
                fontSizeInFloat / minReadableFontSize
            );
            if (coordinateStyle.normal.textColor.a > minVisibleAlpha)
            {
                Handles.BeginGUI();

                GUI.Label(coordinateStyle.padding.Add(rect), textContent, coordinateStyle);

                Handles.EndGUI();
            }
        }
    }
}