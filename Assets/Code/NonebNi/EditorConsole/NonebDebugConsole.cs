using System.Text;
using NonebNi.EditorConsole.Parsers;

namespace NonebNi.EditorConsole
{
    public class NonebDebugConsole
    {
        private readonly CommandHandler _commandHandler;
        private readonly TextLexer _lexer;
        private readonly ExpressionParser _parser;

        public StringBuilder AccumulatedOutput { get; } = new StringBuilder();

        public NonebDebugConsole(CommandHandler commandHandler, ExpressionParser parser, TextLexer lexer)
        {
            _commandHandler = commandHandler;
            _parser = parser;
            _lexer = lexer;
        }

        public void InterpretInput(string input)
        {
            var expressions = _lexer.Lex(input);
            var command = _parser.Parse(expressions);

            _commandHandler.Handle(command, AccumulatedOutput);
        }
    }
}