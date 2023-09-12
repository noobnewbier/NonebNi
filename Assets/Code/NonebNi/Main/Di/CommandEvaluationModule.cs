using NonebNi.Core.Actions;
using NonebNi.Core.Commands;
using NonebNi.Core.Commands.Handlers;
using NonebNi.Core.FlowControl;
using StrongInject;

namespace NonebNi.Main.Di
{
    [Register(typeof(DamageCommandHandler), typeof(ICommandHandler<DamageCommand>))]
    [Register(typeof(TeleportCommandHandler), typeof(ICommandHandler<TeleportCommand>))]
    [Register(typeof(EndTurnCommandHandler), typeof(ICommandHandler<EndTurnCommand>))]
    [Register(typeof(ActionCommandHandler), typeof(ICommandHandler<ActionCommand>))]
    [Register(typeof(CommandEvaluationService), typeof(ICommandEvaluationService))]
    [Register(typeof(TargetValidityChecker), typeof(ITargetValidityChecker))]
    public class CommandEvaluationModule { }
}