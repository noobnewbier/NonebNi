using Code.NonebNi.EditorComponent.Entities;

namespace NonebNi.Editors.Level.Error
{
    public class ErrorEntry
    {
        public readonly string Description;
        public readonly Entity ErrorSource;

        public ErrorEntry(Entity errorSource, string description)
        {
            ErrorSource = errorSource;
            Description = description;
        }
    }
}