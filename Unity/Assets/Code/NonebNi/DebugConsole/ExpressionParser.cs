using System;
using System.Collections.Generic;
using System.Linq;
using NonebNi.DebugConsole.Commands;
using NonebNi.DebugConsole.Expressions;

namespace NonebNi.DebugConsole
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
            if (firstExpression is not StringExpression commandName ||
                !_commandsDataRepository.TryGetCommand(commandName.StringValue, out var data))
                return new ErrorMessageConsoleCommand("invalid input - no recognized command name");

            var commandArgs = expressionArray.Skip(1).ToArray();
            foreach (var constructorInfo in data.CommandType.GetConstructors())
            {
                var constructorArgTypes = constructorInfo.GetParameters().Select(p => p.ParameterType).ToArray();
                var (isSuccess, arguments) = GetArguments(constructorArgTypes, commandArgs);

                if (isSuccess)
                    return (IConsoleCommand)constructorInfo.Invoke(arguments);
            }

            //can't find matching constructor - user provided non-matching arguments -> print help message to provide hint
            return new HelpCommand(commandName.StringValue);
        }

        private static (bool success, object[] arguments) GetArguments(Type[] constructorArgTypes, Expression[] expressions)
        {
            if (constructorArgTypes.Length != expressions.Length) return (false, Array.Empty<object>());

            var toReturn = new object[constructorArgTypes.Length];
            for (var i = 0; i < constructorArgTypes.Length; i++)
            {
                var requiredArgType = constructorArgTypes[i];
                var expression = expressions[i];
                if (!expression.ConvertableTypes.Contains(requiredArgType)) return (false, Array.Empty<object>());

                toReturn[i] = expression.ConvertTo(requiredArgType);
            }

            return (true, toReturn);
        }
    }
}