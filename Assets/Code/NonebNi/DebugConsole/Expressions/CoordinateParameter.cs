using System;
using System.Text.RegularExpressions;
using NonebNi.Core.Coordinates;

namespace NonebNi.DebugConsole.Expressions
{
    public class CoordinateParameter : Expression
    {
        public static readonly Regex Pattern = new(
            @$"\( *{IntParameter.Pattern} *, *{IntParameter.Pattern} *\)"
        );

        private readonly Coordinate _value;

        public CoordinateParameter(Coordinate coordinate, string input) : base(input)
        {
            _value = coordinate;
        }

        public override Type[] ConvertableTypes => new[] { typeof(Coordinate) };

        public override object ConvertTo(Type type)
        {
            if (type != typeof(Coordinate)) throw new InvalidOperationException();

            return _value;
        }
    }
}