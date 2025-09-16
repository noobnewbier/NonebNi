using System;

namespace NonebNi.DebugConsole.Expressions
{
    public class UndefinedExpression : Expression
    {
        public UndefinedExpression(string stringRepresentation) : base(stringRepresentation) { }

        public override Type[] ConvertableTypes =>
            new[] { typeof(UndefinedType) }; //only convertable to UndefinedType - meaning any input with an unknown expression won't match any available command

        public override object ConvertTo(Type type) => throw new InvalidOperationException();

        private class UndefinedType { }
    }
}