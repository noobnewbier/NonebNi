using System;
using System.Text.RegularExpressions;

namespace NonebNi.EditorConsole.Expressions
{
    public class StringExpression : Expression
    {
        public static readonly Regex Pattern = new(@"[a-zA-Z]+");

        public StringExpression(string value) : base(value)
        {
            StringValue = value;
        }

        public override Type ConvertableType => typeof(string);
        public override object Value => StringValue;
        public string StringValue { get; }
    }
}