using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.FlowControl;
using StrongInject;

namespace NonebNi.Main.Di
{
    [Register(typeof(EndTurnCommandHandler), typeof(ICommandHandler<EndTurnCommand>))]
    [Register(typeof(ActionCommandHandler), typeof(ICommandHandler<ActionCommand>))]
    [Register(typeof(CommandEvaluationService), typeof(ICommandEvaluationService))]
    [Register(typeof(TargetFinder), typeof(ITargetFinder))]
    public class CommandEvaluationModule { }
}