using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole.Parsers
{
    public partial class ExpressionParser
    {
        private static IConsoleCommand? ParseDamageConsoleCommand(IReadOnlyList<Expression> expressions)
        {
            if (!(expressions.First() is CommandName { Value: "damage" })) return null;

            if (expressions.Count != 3) return new ErrorMessageConsoleCommand(@"invalid format for ""damage""");

            if (!(expressions[1] is CoordinateParameter coordinateParameter))
                return new ErrorMessageConsoleCommand(@"invalid format for ""damage""");

            if (!(expressions[2] is IntParameter damageParam))
                return new ErrorMessageConsoleCommand(@"invalid format for ""damage""");

            return new DamageConsoleCommand(coordinateParameter.Value, damageParam.Value);
        }
    }
}