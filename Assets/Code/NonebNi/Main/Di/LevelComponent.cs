using System;
using NonebNi.Core.Decision;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;
using NonebNi.Core.Sequences;

namespace NonebNi.Main.Di
{
    public interface ILevelComponent
    {
        LevelData GetLevelData();
        ILevelFlowController GetLevelFlowController();
        ICommandEvaluationService GetCommandEvaluationService();
        ISequencePlayer GetSequencePlayer();
        IAgentDecisionService GetAgentDecisionService();
        IUnitTurnOrderer GetUnitTurnOrderer();
    }

    public class LevelComponent : ILevelComponent
    {
        private readonly Lazy<ICommandEvaluationService> _lazyCommandEvaluationService;
        private readonly Lazy<ILevelFlowController> _lazyLevelFlowController;

        private readonly Lazy<IAgentDecisionService> _lazyAgentDecisionService;
        private readonly Lazy<IDecisionValidator> _lazyDecisionValidator;
        private readonly Lazy<ISequencePlayer> _lazySequencePlayer;
        private readonly Lazy<IUnitTurnOrderer> _lazyUnitTurnOrderer;

        private readonly LevelModule _levelModule;

        public LevelComponent(LevelModule levelModule)
        {
            _levelModule = levelModule;

            _lazyDecisionValidator = new Lazy<IDecisionValidator>(levelModule.GetDecisionValidator);
            _lazyAgentDecisionService = new Lazy<IAgentDecisionService>(levelModule.GetAgentDecisionService);
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

        public ILevelFlowController GetLevelFlowController() => _lazyLevelFlowController.Value;
        public ICommandEvaluationService GetCommandEvaluationService() => _lazyCommandEvaluationService.Value;
        public ISequencePlayer GetSequencePlayer() => _lazySequencePlayer.Value;
        public IAgentDecisionService GetAgentDecisionService() => _lazyAgentDecisionService.Value;
        public IUnitTurnOrderer GetUnitTurnOrderer() => _lazyUnitTurnOrderer.Value;
    }
}