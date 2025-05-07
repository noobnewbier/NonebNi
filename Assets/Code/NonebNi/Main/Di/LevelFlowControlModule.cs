using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Entities;
using NonebNi.Ui.Sequences;
using StrongInject;

namespace NonebNi.Main.Di
{
    [Register(typeof(LevelFlowController), typeof(ILevelFlowController))]
    [Register(typeof(EntityRepository), typeof(IEntityRepository))]
    [Register(typeof(UnitTurnOrderer), typeof(IUnitTurnOrderer))]
    [Register(typeof(SequencePlayer), typeof(ISequencePlayer))]
    [Register(typeof(DecisionValidator), typeof(IDecisionValidator))]
    [Register(typeof(PathfindingService), typeof(IPathfindingService))]
    [Register(typeof(GameEventControl), typeof(IGameEventControl))]
    [RegisterModule(typeof(CommandEvaluationModule))]
    public class LevelFlowControlModule { }
}