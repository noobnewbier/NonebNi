using NonebNi.Core.Maps;

namespace NonebNi.LevelEditor.Level.Maps
{
    public class GridPresenter
    {
        private readonly GridView _gridView;
        private readonly NonebEditorModel _nonebEditorModel;

        public GridPresenter(GridView gridView, NonebEditorModel nonebEditorModel, IReadOnlyMap map)
        {
            _gridView = gridView;
            _nonebEditorModel = nonebEditorModel;
            Map = map;
        }

        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;
        public IReadOnlyMap Map { get; }
    }
}