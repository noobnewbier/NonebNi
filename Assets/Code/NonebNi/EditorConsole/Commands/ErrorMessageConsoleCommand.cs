namespace NonebNi.EditorConsole.Commands
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