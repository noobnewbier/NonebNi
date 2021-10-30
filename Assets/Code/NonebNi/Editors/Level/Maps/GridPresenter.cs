using NonebNi.Core.Maps;

namespace NonebNi.Editors.Level.Maps
{
    public class GridPresenter
    {
        private readonly GridView _gridView;
        private readonly LevelEditorModel _levelEditorModel;
        private readonly NonebEditorModel _nonebEditorModel;

        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;
        public IReadOnlyMap Map => _levelEditorModel.Map;

        public GridPresenter(GridView gridView, LevelEditorModel levelEditorModel, NonebEditorModel nonebEditorModel)
        {
            _gridView = gridView;
            _levelEditorModel = levelEditorModel;
            _nonebEditorModel = nonebEditorModel;
        }
    }
}