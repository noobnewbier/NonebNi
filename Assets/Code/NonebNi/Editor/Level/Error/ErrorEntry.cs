using NonebNi.Core.Entities;

namespace NonebNi.Editor.Level.Error
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