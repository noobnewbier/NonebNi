using System.Linq;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using UnityEditor;
using UnityEngine;

namespace NonebNi.Editor.Maps.Grid
{
    public class GridView
    {
        private bool _isDrawing;
        private Map? _map;
        private GridPresenter _presenter;
        private WorldConfigData? _worldConfig;

        public GridView()
        {
            _presenter = new GridPresenter(this);
        }

        public void StartDrawGridWithData(Map map, WorldConfigData worldConfig)
        {
            _isDrawing = true;
            _worldConfig = worldConfig;
            _map = map;
        }

        public void StopDrawing()
        {
            _isDrawing = false;
        }

        public void DrawGrid()
        {
            if (!_isDrawing) return;
            if (_map == null) return;
            if (_worldConfig == null) return;

            var grid = _map.GetGridForType<Tile>();
            foreach (var tile in grid)
            {
                if (tile == null) continue;

                var center = GetTilePosition(_worldConfig.MapStartingPosition.y, tile.Coordinate, _worldConfig);
                // Handles.Label(center, $"{tile.Coordinate}", _coordinateStyle);
                DrawCenteredLabel(center, $"{tile.Coordinate}", _worldConfig.InnerRadius);
                var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
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
            var coordinateStyle = new GUIStyle {alignment = TextAnchor.MiddleCenter};
            var textContent = EditorGUIUtility.TrTempContent(text);
            var size = coordinateStyle.CalcSize(textContent);
            var rect = new Rect(guiPoint, size);
            rect.xMin -= size.x / 2;
            rect.xMax -= size.x / 2;
            rect.yMin -= size.y / 2;
            rect.yMax -= size.y / 2;

            //stop drawing the label when the text is so large, it goes "out of the bounds"(in our use case, the tile itself)
            //A text goes out of bounds when the min/max point distance to center is greater than the maximum offset
            var minAcceptableX = HandleUtility.WorldToGUIPoint(position + Vector3.left * maxOffsetFromCenter).x;
            var maxAcceptableX = HandleUtility.WorldToGUIPoint(position + Vector3.right * maxOffsetFromCenter).x;
            var minAcceptableY = HandleUtility.WorldToGUIPoint(position + Vector3.back * maxOffsetFromCenter).y;
            var maxAcceptableY = HandleUtility.WorldToGUIPoint(position + Vector3.forward * maxOffsetFromCenter).y;
            if (!(minAcceptableX < rect.xMin || maxAcceptableX > rect.xMax || minAcceptableY < rect.yMin || maxAcceptableY > rect.yMax))
                //todo: better handle cases where the camera is far from the label
                return;

            Handles.BeginGUI();

            GUI.Label(coordinateStyle.padding.Add(rect), textContent, coordinateStyle);

            Handles.EndGUI();
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