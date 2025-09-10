using System;

namespace NonebNi.DebugConsole.Expressions
{
    public abstract class Expression
    {
        public Expression(string stringRepresentation)
        {
            StringRepresentation = stringRepresentation;
        }

        public string StringRepresentation { get; }

        public abstract Type ConvertableType { get; }

        public abstract object Value { get; }
    }
}