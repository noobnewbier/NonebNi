using System;

namespace NonebNi.LevelEditor.Di
{
    public interface INonebEditorComponent
    {
        NonebEditorModel NonebEditorModel { get; }
        NonebEditor NonebEditor { get; }
    }

    public class NonebEditorComponent : INonebEditorComponent
    {
        private readonly Lazy<NonebEditor> _lazyEditor;
        private readonly Lazy<NonebEditorModel> _lazyEditorModel;

        public NonebEditorComponent(NonebEditorModule module)
        {
            _lazyEditorModel = new Lazy<NonebEditorModel>(module.GetNonebEditorModel);
            _lazyEditor = new Lazy<NonebEditor>(module.GetNonebEditor);
        }

        public NonebEditorModel NonebEditorModel => _lazyEditorModel.Value;
        public NonebEditor NonebEditor => _lazyEditor.Value;
    }
}