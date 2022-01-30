using System.Text.RegularExpressions;

namespace NonebNi.EditorConsole.Expressions
{
    public class CommandName : Expression
    {
        public static readonly Regex Pattern = new Regex(@"[a-zA-Z]+");
        public string Value { get; }

        public CommandName(string value) : base(value)
        {
            Value = value;
        }
    }
}