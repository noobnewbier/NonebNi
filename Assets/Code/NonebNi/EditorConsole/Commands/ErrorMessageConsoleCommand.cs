namespace NonebNi.EditorConsole.Commands
{
    public class ErrorMessageConsoleCommand : IConsoleCommand
    {
        public string Message { get; }

        public ErrorMessageConsoleCommand(string message)
        {
            Message = message;
        }
    }
}