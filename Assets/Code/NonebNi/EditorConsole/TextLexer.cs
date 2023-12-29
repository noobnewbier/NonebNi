using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NonebNi.Core.Coordinates;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole
{
    public class TextLexer
    {
        public IEnumerable<Expression> Lex(string input)
        {
            do
            {
                if (TryMatchFirstArg(StringExpression.Pattern, input, out var commandNameMatch))
                {
                    input = input.Remove(0, commandNameMatch.Length);
                    var commandName = commandNameMatch.Value;

                    yield return new StringExpression(commandName);
                }
                else if (TryMatchFirstArg(ArrayExpression.Pattern, input, out var arrayMatch))
                {
                    input = input.Remove(0, arrayMatch.Length);
                    var arrayContent = arrayMatch.Value[1..^1];
                    var expressions = Lex(arrayContent).ToArray();

                    yield return new ArrayExpression(
                        expressions,
                        arrayMatch.Value
                    );
                }
                else if (TryMatchFirstArg(CoordinateParameter.Pattern, input, out var coordinateMatch))
                {
                    input = input.Remove(0, coordinateMatch.Length);
                    var coordinateToSpaceSeparatedInts =
                        coordinateMatch.Value.Replace("(", " ").Replace(")", " ").Replace(",", " ");
                    var intExpressions = Lex(coordinateToSpaceSeparatedInts).Cast<IntParameter>().ToArray();

                    yield return new CoordinateParameter(
                        new Coordinate(intExpressions[0].IntValue, intExpressions[2].IntValue),
                        coordinateMatch.Value
                    );
                }
                else if (TryMatchFirstArg(IntParameter.Pattern, input, out var intMatch))
                {
                    input = input.Remove(0, intMatch.Length);

                    yield return new IntParameter(int.Parse(intMatch.Value));
                }
                else
                {
                    //can't match anything - it's an undefined expression
                    yield return new UndefinedExpression(input);

                    input = string.Empty;
                }

                input = input.TrimStart();
            } while (input.Any());
        }

        private static bool TryMatchFirstArg(Regex regex, string input, out Match match)
        {
            match = regex.Match(input);

            return match.Success && match.Index == 0;
        }
    }
}