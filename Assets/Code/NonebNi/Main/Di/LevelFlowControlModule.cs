using NonebNi.Core.Decision;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Entities;
using NonebNi.Ui.Sequences;
using StrongInject;

namespace NonebNi.Main.Di
{
    [Register(typeof(LevelFlowController), Scope.SingleInstance, typeof(ILevelFlowController))]
    [Register(typeof(EntityRepository), typeof(IEntityRepository))]
    [Register(typeof(UnitTurnOrderer), typeof(IUnitTurnOrderer))]
    [Register(typeof(SequencePlayer), typeof(ISequencePlayer))]
    [Register(typeof(DecisionValidator), typeof(IDecisionValidator))]
    [RegisterModule(typeof(CommandEvaluationModule))]
    public class LevelFlowControlModule { }
}