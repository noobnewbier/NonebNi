using System.Text.RegularExpressions;
using NonebNi.Core.Coordinates;

namespace NonebNi.EditorConsole.Expressions
{
    public class CoordinateParameter : Expression
    {
        public static readonly Regex Pattern = new Regex(
            @$"\( *{IntParameter.Pattern} *, *{IntParameter.Pattern} *, *{IntParameter.Pattern} *\)"
        );

        public Coordinate Value { get; }

        public CoordinateParameter(Coordinate coordinate, string input) : base(input)
        {
            Value = coordinate;
        }
    }
}