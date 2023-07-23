using System;
using System.Text.RegularExpressions;

namespace NonebNi.EditorConsole.Expressions
{
    public class IntParameter : Expression
    {
        public static readonly Regex Pattern = new(@"(\+|-| )?[0-9]+");

        public IntParameter(int value) : base(value.ToString())
        {
            IntValue = value;
        }

        public override Type ConvertableType => typeof(int);
        public override object Value => IntValue;
        public int IntValue { get; }
    }
}