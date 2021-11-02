using NonebNi.Core.Coordinates;
using NonebNi.Core.Level;
using UnityEngine;

namespace NonebNi.Editors.Level.Inspector
{
    public class TileInspectorPresenter
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;
        private readonly NonebEditorModel _nonebEditorModel;
        private readonly TileInspectorView _view;

        public bool IsDrawing => _nonebEditorModel.IsHelperWindowsVisible;

        public TileInspectorPresenter(TileInspectorView view,
                                      CoordinateAndPositionService coordinateAndPositionService,
                                      NonebEditorModel nonebEditorModel)
        {
            _view = view;
            _nonebEditorModel = nonebEditorModel;
            _coordinateAndPositionService = coordinateAndPositionService;
        }

        public Coordinate FindCoordinate(Vector3 worldPos) =>
            _coordinateAndPositionService.NearestCoordinateForPoint(worldPos);
    }
}