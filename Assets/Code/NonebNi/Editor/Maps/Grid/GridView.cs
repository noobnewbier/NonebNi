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
                DrawCenteredLabel(center, $"{tile.Coordinate}");
                var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
            }
        }

        /// <summary>
        /// <see cref="Handles.Label(UnityEngine.Vector3,string)" /> doesn't seems to be working with centered text properly.
        /// My speculation is that <see cref="Handles.Label(UnityEngine.Vector3,string)" /> is creating a temporary <see cref="GUIContent" /> through a method
        /// <see cref="EditorGUIUtility.TempContent(string)" /> and for some reason this leads calculating the rect width correctly,
        /// and hence the text box is not offset correctly.
        /// This implementation is basically copying the internals of <see cref="Handles.Label(UnityEngine.Vector3,string)" />, except we didn't use the
        /// temporary GUI Content
        /// Any how using this should do the trick.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private void DrawCenteredLabel(Vector3 position, string text)
        {
            //behind the camera
            if (HandleUtility.WorldToGUIPointWithDepth(position).z < 0.0)
                return;

            Handles.BeginGUI();
            GUIStyle coordinateStyle = new GUIStyle {alignment = TextAnchor.MiddleCenter};
            var guiPoint = HandleUtility.WorldToGUIPoint(position);
            var textContent = EditorGUIUtility.TrTempContent(text);
            var size = coordinateStyle.CalcSize(textContent);
            var rect = new Rect(guiPoint, size);
            rect.xMin -= size.x / 2;
            rect.xMax -= size.x / 2;
            rect.yMin -= size.y / 2;
            rect.yMax -= size.y / 2;
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