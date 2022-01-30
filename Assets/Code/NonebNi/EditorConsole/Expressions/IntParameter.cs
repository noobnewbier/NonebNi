using System.Text.RegularExpressions;

namespace NonebNi.EditorConsole.Expressions
{
    public class IntParameter : Expression
    {
        public static readonly Regex Pattern = new Regex(@"(\+|-| )?[0-9]+");
        public int Value { get; }

        public IntParameter(int value) : base(value.ToString())
        {
            Value = value;
        }
    }
}