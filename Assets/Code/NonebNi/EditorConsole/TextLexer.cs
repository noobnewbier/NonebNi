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
                if (TryMatchFirstArg(CommandName.Pattern, input, out var commandNameMatch))
                {
                    input = input.Remove(0, commandNameMatch.Length);
                    var commandName = commandNameMatch.Value;

                    yield return new CommandName(commandName);
                }
                else if (TryMatchFirstArg(CoordinateParameter.Pattern, input, out var coordinateMatch))
                {
                    input = input.Remove(0, coordinateMatch.Length);
                    var coordinateToSpaceSeparatedInts =
                        coordinateMatch.Value.Replace("(", " ").Replace(")", " ").Replace(",", " ");
                    var intExpressions = Lex(coordinateToSpaceSeparatedInts).Cast<IntParameter>().ToArray();

                    yield return new CoordinateParameter(
                        new Coordinate(intExpressions[0].Value, intExpressions[2].Value),
                        coordinateMatch.Value
                    );
                }
                else if (TryMatchFirstArg(IntParameter.Pattern, input, out var intMatch))
                {
                    input = input.Remove(0, intMatch.Length);

                    yield return new IntParameter(int.Parse(intMatch.Value));
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