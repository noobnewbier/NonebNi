using System.Linq;
using Code.NonebNi.Game.Level;
using NonebNi.Editors.Di;
using UnityEditor;
using UnityEngine.Rendering;

namespace NonebNi.Editors.Level.Maps
{
    public class GridView
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;

        //We need to reuse the same GUIContent for all labels, otherwise it is generating way too much GC per frame

        private readonly GridPresenter _presenter;
        private readonly WorldConfigData _worldConfig;

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
            var coordinates = _presenter.Map.GetAllCoordinates();

            foreach (var coordinate in coordinates)
            {
                if (_presenter.Map.TryGet(coordinate, out _))
                {
                    var center = _coordinateAndPositionService.FindPosition(coordinate);

                    var corners = _worldConfig.TileCornersOffset.Select(c => center + c).ToList();
                    Handles.DrawLine(corners[0], corners[5]);
                    for (var i = 0; i < corners.Count - 1; i++) Handles.DrawLine(corners[i], corners[i + 1]);
                }
            }

            Handles.zTest = originalZTest;
        }
    }
}