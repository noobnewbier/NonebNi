using System;
using System.Linq;
using NonebNi.Core.Agents;
using NonebNi.Core.Decision;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Sequences;

namespace NonebNi.Main.Di
{
    public interface ILevelComponent
    {
        LevelData GetLevelData();
        IPlayerAgent GetPlayerAgent();
        ILevelFlowController GetLevelFlowController();
        ICommandEvaluationService GetCommandEvaluationService();
        ISequencePlayer GetSequencePlayer();
        IAgentsService GetAgentsService();
        IUnitTurnOrderer GetUnitTurnOrderer();
    }

    public class LevelComponent : ILevelComponent
    {
        private readonly IAgent[] _agents;

        private readonly Lazy<IAgentsService> _lazyAgentDecisionService;
        private readonly Lazy<ICommandEvaluationService> _lazyCommandEvaluationService;
        private readonly Lazy<IDecisionValidator> _lazyDecisionValidator;
        private readonly Lazy<ILevelFlowController> _lazyLevelFlowController;
        private readonly Lazy<ISequencePlayer> _lazySequencePlayer;
        private readonly Lazy<IUnitTurnOrderer> _lazyUnitTurnOrderer;

        private readonly LevelModule _levelModule;

        public LevelComponent(LevelModule levelModule, IAgent[] agents)
        {
            _levelModule = levelModule;
            _agents = agents;

            _lazyDecisionValidator = new Lazy<IDecisionValidator>(levelModule.GetDecisionValidator);
            _lazyAgentDecisionService = new Lazy<IAgentsService>(() => levelModule.GetAgentsService(agents));
            _lazyUnitTurnOrderer = new Lazy<IUnitTurnOrderer>(levelModule.GetUnitTurnOrderer);
            _lazyCommandEvaluationService = new Lazy<ICommandEvaluationService>(levelModule.GetCommandEvaluationService);
            _lazySequencePlayer = new Lazy<ISequencePlayer>(levelModule.GetSequencePlayer);
            _lazyLevelFlowController = new Lazy<ILevelFlowController>(
                () => levelModule.GetLevelFlowController(
                    GetCommandEvaluationService(),
                    _lazyUnitTurnOrderer.Value,
                    _lazyAgentDecisionService.Value,
                    GetSequencePlayer(),
                    _lazyDecisionValidator.Value
                )
            );
        }

        public LevelData GetLevelData() => _levelModule.LevelData;
        public IPlayerAgent GetPlayerAgent() => _agents.OfType<IPlayerAgent>().First();

        public ILevelFlowController GetLevelFlowController() => _lazyLevelFlowController.Value;
        public ICommandEvaluationService GetCommandEvaluationService() => _lazyCommandEvaluationService.Value;
        public ISequencePlayer GetSequencePlayer() => _lazySequencePlayer.Value;
        public IAgentsService GetAgentsService() => _lazyAgentDecisionService.Value;
        public IUnitTurnOrderer GetUnitTurnOrderer() => _lazyUnitTurnOrderer.Value;
    }
}