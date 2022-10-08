using System;

namespace NonebNi.EditorConsole.Commands.Attributes
{
    /// <summary>
    /// Providing additional information for a specific signature of a command, optional
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public class SignatureInfoAttribute : Attribute
    {
        public readonly string Description;

        public SignatureInfoAttribute(string description)
        {
            Description = description;
        }
    }
}