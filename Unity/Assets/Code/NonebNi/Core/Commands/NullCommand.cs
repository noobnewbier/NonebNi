namespace NonebNi.Core.Commands
{
    public class NullCommand : ICommand
    {
        public static ICommand Instance { get; } = new NullCommand();
    }
}