using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Core.Tiles;
using NonebNi.Core.Units;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Level.Map
{
    public class MapView
    {
        private Core.Maps.Map? _map;
        private MapPresenter _presenter;
        private WorldConfigData? _worldConfig;
        public bool IsDrawingGrid { private get; set; }
        public bool IsDrawingGizmos { private get; set; }

        public MapView()
        {
            _presenter = new MapPresenter(this);
        }

        public void SetUpData(Core.Maps.Map map, WorldConfigData worldConfig)
        {
            _worldConfig = worldConfig;
            _map = map;
        }

        public void OnSceneDraw()
        {
            DrawGrid();
            DrawGizmos();
        }

        private void DrawGrid()
        {
            if (!IsDrawingGrid) return;
            if (_map == null) return;
            if (_worldConfig == null) return;

            var grid = _map.GetGridForType<Tile>();
            foreach (var tile in grid)
            {
                if (tile == null) continue;

                var center = GetTilePosition(_worldConfig.MapStartingPosition.y, tile.Coordinate, _worldConfig);

                var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
            }
        }

        private void DrawGizmos()
        {
            if (!IsDrawingGizmos) return;
            if (_map == null) return;
            if (_worldConfig == null) return;

            var grid = _map.GetGridForType<Tile>();
            foreach (var tile in grid)
            {
                if (tile == null) continue;

                var centerPosition = GetTilePosition(_worldConfig.MapStartingPosition.y, tile.Coordinate, _worldConfig);
                var unit = _map.Get<Unit>(tile.Coordinate);

                var text = $"{tile.Coordinate}";
                if (unit != null) text += $"\n Unit: {unit.Data.Name}";
                DrawCenteredLabel(centerPosition, text, _worldConfig.InnerRadius);
            }
        }

        /// <summary>
        /// <see cref="Handles.Label(UnityEngine.Vector3,string)" /> can't center the label properly.
        /// Root of the problem is that <see cref="HandleUtility.WorldPointToSizedRect" /> isn't taking rects maxX/Y into account,
        /// This leads to a problem where the rect is essentially stretched to the left a bit, and as a result when trying to center the item,
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

        private Vector3 GetTilePosition(float yPosition, Coordinate coordinate, WorldConfigData worldConfig)
        {
            var upDistance = worldConfig.OuterRadius * 1.5f;
            var sideDistance = worldConfig.InnerRadius * 2f;
            var sideOffset = coordinate.Z % 2 * sideDistance / 2f;

            return new Vector3(coordinate.X * sideDistance + sideOffset, yPosition, coordinate.Z * upDistance) + worldConfig.MapStartingPosition;
        }
    }
}