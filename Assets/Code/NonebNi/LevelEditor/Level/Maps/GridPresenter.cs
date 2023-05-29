﻿namespace NonebNi.LevelEditor.Level.Maps
{
    public class GridPresenter
    {
        private readonly GridView _gridView;
        private readonly LevelEditorModel _levelEditorModel;
        private readonly NonebEditorModel _nonebEditorModel;

        public GridPresenter(GridView gridView, LevelEditorModel levelEditorModel, NonebEditorModel nonebEditorModel)
        {
            _gridView = gridView;
            _levelEditorModel = levelEditorModel;
            _nonebEditorModel = nonebEditorModel;
        }

        public bool IsDrawingGrid => _nonebEditorModel.IsGridVisible;
        public IEditorMap Map => _levelEditorModel.EditorMap;
    }
}