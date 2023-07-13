using NonebNi.Core.Coordinates;
using NonebNi.Terrain;
using UnityEngine;

namespace NonebNi.LevelEditor.Level.Tiles
{
    public class TileInspectorPresenter
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;
        private readonly NonebEditorModel _nonebEditorModel;
        private readonly TileInspectorView _view;

        public TileInspectorPresenter(TileInspectorView view,
            ICoordinateAndPositionService coordinateAndPositionService,
            NonebEditorModel nonebEditorModel)
        {
            _view = view;
            _nonebEditorModel = nonebEditorModel;
            _coordinateAndPositionService = coordinateAndPositionService;
        }

        public bool IsDrawing => _nonebEditorModel.IsHelperWindowsVisible;

        public Coordinate FindCoordinate(Vector3 worldPos) =>
            _coordinateAndPositionService.NearestCoordinateForPoint(worldPos);
    }
}