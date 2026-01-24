using NonebNi.DebugConsole;
using StrongInject;

namespace NonebNi.Main.Di.Debug
{
    [
        Register(typeof(NonebDebugConsole)),
        Register(typeof(CommandHandler)),
        Register(typeof(ExpressionParser)),
        Register(typeof(TextLexer)),
        Register(typeof(CommandsDataRepository), typeof(ICommandsDataRepository))
    ]
    public class NonebDebugConsoleModule { }
}