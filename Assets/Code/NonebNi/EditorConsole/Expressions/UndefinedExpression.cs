using System;

namespace NonebNi.EditorConsole.Expressions
{
    public class UndefinedExpression : Expression
    {
        public UndefinedExpression(string stringRepresentation) : base(stringRepresentation) { }

        public override Type ConvertableType =>
            typeof(UndefinedType); //only convertable to UndefinedType - meaning any input with an unknown expression won't match any available command

        public override object Value => new UndefinedType();

        private class UndefinedType { }
    }
}