using NonebNi.EditorComponent.Entities;

namespace NonebNi.Editors.Level.Error
{
    public class ErrorEntry
    {
        public readonly string Description;
        public readonly EditorEntity ErrorSource;

        public ErrorEntry(EditorEntity errorSource, string description)
        {
            ErrorSource = errorSource;
            Description = description;
        }
    }
}