using System;

namespace NonebNi.Editor.Di
{
    public interface INonebEditorComponent
    {
        NonebEditorModel NonebEditorModel { get; }
    }

    public class NonebEditorComponent : INonebEditorComponent
    {
        private readonly Lazy<NonebEditorModel> _lazyEditorModel;

        public NonebEditorComponent(NonebEditorModule module)
        {
            _lazyEditorModel = new Lazy<NonebEditorModel>(module.GetNonebEditorModel);
        }

        public NonebEditorModel NonebEditorModel => _lazyEditorModel.Value;
    }
}