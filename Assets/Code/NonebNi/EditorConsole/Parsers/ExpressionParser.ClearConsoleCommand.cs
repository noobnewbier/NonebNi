using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole.Parsers
{
    public partial class ExpressionParser
    {
        private static IConsoleCommand? ParseClearConsoleCommand(IReadOnlyList<Expression> expressions)
        {
            if (!(expressions.First() is CommandName { Value: "clear" })) return null;

            if (expressions.Count != 1) return new ErrorMessageConsoleCommand(@"invalid format for ""clear""");

            return new ClearConsoleCommand();
        }
    }
}