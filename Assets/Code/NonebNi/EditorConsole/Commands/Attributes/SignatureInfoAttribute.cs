using System;

namespace NonebNi.EditorConsole.Commands.Attributes
{
    public class SignatureInfoAttribute : Attribute
    {
        public readonly string Description;

        public SignatureInfoAttribute(string description)
        {
            Description = description;
        }
    }
}