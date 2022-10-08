using System.Collections.Generic;
using System.Linq;
using NonebNi.EditorConsole.Commands;
using NonebNi.EditorConsole.Expressions;

namespace NonebNi.EditorConsole.Parsers
{
    public class ExpressionParser
    {
        private readonly ICommandsDataRepository _commandsDataRepository;

        public ExpressionParser(ICommandsDataRepository commandsDataRepository)
        {
            _commandsDataRepository = commandsDataRepository;
        }

        public IConsoleCommand Parse(IEnumerable<Expression> expressions)
        {
            var expressionArray = expressions as Expression[] ?? expressions.ToArray();
            if (!expressionArray.Any()) return new ErrorMessageConsoleCommand("invalid input");

            var firstExpression = expressionArray[0];
            if (!(firstExpression is StringExpression commandName) ||
                !_commandsDataRepository.TryGetCommand(commandName.StringValue, out var data))
                return new ErrorMessageConsoleCommand("invalid input - no recognized command name");

            var commandArgs = expressionArray.Skip(1).ToArray();
            foreach (var constructorInfo in data.CommandType.GetConstructors())
            {
                var constructorArgTypes = constructorInfo.GetParameters().Select(p => p.ParameterType).ToArray();
                var isConstructorMatchingArguments =
                    constructorArgTypes.SequenceEqual(commandArgs.Select(a => a.ConvertableType));
                if (isConstructorMatchingArguments)
                    return (IConsoleCommand)constructorInfo.Invoke(commandArgs.Select(a => a.Value).ToArray());
            }

            //can't find matching constructor - user provided non-matching arguments -> print help message to provide hint
            return new HelpCommand(commandName.StringValue);
        }
    }
}