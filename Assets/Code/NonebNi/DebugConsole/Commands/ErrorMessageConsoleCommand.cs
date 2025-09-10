namespace NonebNi.DebugConsole.Commands
{
    public class ErrorMessageConsoleCommand : IConsoleCommand
    {
        public ErrorMessageConsoleCommand(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}