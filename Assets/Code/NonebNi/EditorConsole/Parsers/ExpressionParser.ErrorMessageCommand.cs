using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole.Parsers
{
    public partial class ExpressionParser
    {
        private static IConsoleCommand ParseErrorMessageCommand(IReadOnlyList<Expression> expressions)
        {
            return new ErrorMessageConsoleCommand(
                $"invalid input: {string.Concat(expressions.Select(e => e.StringRepresentation))}"
            );
        }
    }
}