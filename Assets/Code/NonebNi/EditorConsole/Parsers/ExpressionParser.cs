using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole.Parsers
{
    public partial class ExpressionParser
    {
        private readonly CommandParser[] _parserDelegates =
        {
            ParseDamageConsoleCommand,
            ParseClearConsoleCommand,
            ParseErrorMessageCommand
        };

        public IConsoleCommand Parse(IEnumerable<Expression> expressions)
        {
            var expressionArray = expressions as Expression[] ?? expressions.ToArray();
            if (!expressionArray.Any()) return new ErrorMessageConsoleCommand("invalid input");

            foreach (var parserDelegate in _parserDelegates)
            {
                var command = parserDelegate(expressionArray);
                if (command != null) return command;
            }

            return new ErrorMessageConsoleCommand("invalid input");
        }

        private delegate IConsoleCommand? CommandParser(IReadOnlyList<Expression> expressions);
    }
}