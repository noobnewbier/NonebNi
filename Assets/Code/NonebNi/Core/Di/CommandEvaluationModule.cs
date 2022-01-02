using NonebNi.Core.FlowControl;

namespace NonebNi.Core.Di
{
    public class CommandEvaluationModule
    {
        public ICommandEvaluationService GetCommandEvaluationService() => new CommandEvaluationService();
    }
}