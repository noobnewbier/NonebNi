using NonebNi.Core.Commands;

namespace NonebNi.Core.FlowControl
{
    public interface IPlayerDecisionService
    {
        ICommand Command { get; }
        void SetCommand(ICommand command);
    }

    public class PlayerDecisionService : IPlayerDecisionService
    {
        public ICommand Command { get; private set; } = NullCommand.Instance;

        public void SetCommand(ICommand command)
        {
            Command = command;
        }
    }
}