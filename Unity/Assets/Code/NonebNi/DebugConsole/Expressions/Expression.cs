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

        public abstract Type[] ConvertableTypes { get; }

        /*
         * Note:
         * - also the way we do this is kind of hacky, it's likely far easier, to have different expressions that only represent one type, than one expression mapping to various types.
         * It's faster though so god forbids.
         *
         * - One day I will change it so they never throw, but the day I wrote this, is not the day
         */
        public abstract object ConvertTo(Type type);
    }
}