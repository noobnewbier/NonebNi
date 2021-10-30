namespace NonebNi.Editors.Di
{
    public class NonebEditorModule
    {
        private readonly NonebEditor _nonebEditor;

        public NonebEditorModule(NonebEditor nonebEditor)
        {
            _nonebEditor = nonebEditor;
        }

        public NonebEditorModel GetNonebEditorModel() => new NonebEditorModel();

        public NonebEditor GetNonebEditor() => _nonebEditor;
    }
}