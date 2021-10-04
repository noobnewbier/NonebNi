using System.Linq;
using NonebNi.Core.Level;
using NonebNi.Core.Maps;
using NonebNi.Core.Tiles;
using NonebNi.Editor.Di;
using UnityEditor;
using UnityEngine.Rendering;

namespace NonebNi.Editor.Level.Maps
{
    public class GridView
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;

        //We need to reuse the same GUIContent for all labels, otherwise it is generating way too much GC per frame

        private readonly GridPresenter _presenter;
        private readonly WorldConfigData _worldConfig;

        private IReadOnlyMap Map => _presenter.Map;

        public GridView(ILevelEditorComponent component,
                        CoordinateAndPositionService coordinateAndPositionService,
                        WorldConfigData worldConfig)
        {
            _coordinateAndPositionService = coordinateAndPositionService;
            _worldConfig = worldConfig;
            _presenter = component.CreateMapPresenter(this);
        }

        public void OnSceneDraw()
        {
            DrawGrid();
        }

        private void DrawGrid()
        {
            if (!_presenter.IsDrawingGrid) return;

            var originalZTest = Handles.zTest;

            Handles.zTest = CompareFunction.Less;
            var coordinates = Map.GetAllCoordinates();
            foreach (var (coordinate, tile) in coordinates.Select(c => (c, Map.Get<TileData>(c))))
            {
                if (tile == null) continue;

                var center = _coordinateAndPositionService.FindPosition(coordinate);

                var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                Handles.DrawLine(corners[0], corners[5]);
                for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
            }

            Handles.zTest = originalZTest;
        }
    }
}