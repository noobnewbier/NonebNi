using System;
using System.Text.RegularExpressions;

namespace NonebNi.DebugConsole.Expressions
{
    public class IntParameter : Expression
    {
        public static readonly Regex Pattern = new(@"(\+|-| )?[0-9]+");

        public IntParameter(int value) : base(value.ToString())
        {
            IntValue = value;
        }

        public override Type[] ConvertableTypes => new[] { typeof(int) };

        public int IntValue { get; }

        public override object ConvertTo(Type type)
        {
            if (type != typeof(int)) return 0;

            return IntValue;
        }
    }
}