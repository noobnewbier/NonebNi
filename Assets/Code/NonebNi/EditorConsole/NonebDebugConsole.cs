using System.Text;
using Cysharp.Threading.Tasks;

namespace NonebNi.EditorConsole
{
    public class NonebDebugConsole
    {
        private readonly CommandHandler _commandHandler;
        private readonly TextLexer _lexer;
        private readonly ExpressionParser _parser;

        public NonebDebugConsole(CommandHandler commandHandler, ExpressionParser parser, TextLexer lexer)
        {
            _commandHandler = commandHandler;
            _parser = parser;
            _lexer = lexer;
        }

        public StringBuilder AccumulatedOutput { get; } = new();

        public void InterpretInput(string input)
        {
            var expressions = _lexer.Lex(input);
            var command = _parser.Parse(expressions);

            _commandHandler.Handle(command, AccumulatedOutput).Forget();
        }
    }
}