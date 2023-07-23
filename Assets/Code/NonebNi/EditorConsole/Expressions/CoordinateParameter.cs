using System;
using System.Text.RegularExpressions;
using NonebNi.Core.Coordinates;

namespace NonebNi.EditorConsole.Expressions
{
    public class CoordinateParameter : Expression
    {
        public static readonly Regex Pattern = new(
            @$"\( *{IntParameter.Pattern} *, *{IntParameter.Pattern} *, *{IntParameter.Pattern} *\)"
        );

        public CoordinateParameter(Coordinate coordinate, string input) : base(input)
        {
            Value = coordinate;
        }

        public override object Value { get; }

        public override Type ConvertableType => typeof(Coordinate);
    }
}