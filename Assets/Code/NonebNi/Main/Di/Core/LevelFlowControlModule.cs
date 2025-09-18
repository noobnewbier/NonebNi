using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Agents;
using NonebNi.Core.Decisions;
using NonebNi.Core.Factions;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Pathfinding;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Entities;
using NonebNi.Ui.Sequences;
using StrongInject;

namespace NonebNi.Main.Di.Core
{
    [
        Register(typeof(ActionRepository), typeof(IActionRepository)),
        Register(typeof(LevelFlowController), typeof(ILevelFlowController)),
        Register(typeof(EntityRepository), typeof(IEntityRepository)),
        Register(typeof(UnitTurnOrderer), typeof(IUnitTurnOrderer)),
        Register(typeof(SequencePlayer), typeof(ISequencePlayer)),
        Register(typeof(DecisionValidator), typeof(IDecisionValidator)),
        Register(typeof(PathfindingService), typeof(IPathfindingService)),
        Register(typeof(GameEventControl), typeof(IGameEventControl)),
        Register(typeof(AgentsService), typeof(IAgentsService)),
        Register(typeof(FactionService), typeof(IFactionService)),
        RegisterModule(typeof(CommandEvaluationModule))
    ]
    public class LevelFlowControlModule
    {
        [Factory]
        public static IPlayerAgent ProvidePlayerAgent(IAgent[] agents) => agents.OfType<IPlayerAgent>().First();
    }
}