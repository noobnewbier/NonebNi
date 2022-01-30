using System;
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
    }

    public class LevelComponent : ILevelComponent
    {
        private readonly Lazy<ICommandEvaluationService> _lazyCommandEvaluationService;
        private readonly Lazy<ILevelFlowController> _lazyLevelFlowController;

        private readonly Lazy<IPlayerDecisionService> _lazyPlayerDecisionService;
        private readonly Lazy<ISequencePlayer> _lazySequencePlayer;
        private readonly Lazy<IUnitTurnOrderer> _lazyUnitTurnOrderer;

        private readonly LevelModule _levelModule;

        public LevelComponent(LevelModule levelModule)
        {
            _levelModule = levelModule;

            _lazyPlayerDecisionService = new Lazy<IPlayerDecisionService>(levelModule.GetPlayerDecisionService);
            _lazyUnitTurnOrderer = new Lazy<IUnitTurnOrderer>(levelModule.GetUnitTurnOrderer);
            _lazyCommandEvaluationService = new Lazy<ICommandEvaluationService>(levelModule.GetCommandEvaluationService);
            _lazySequencePlayer = new Lazy<ISequencePlayer>(levelModule.GetSequencePlayer);
            _lazyLevelFlowController = new Lazy<ILevelFlowController>(
                () => levelModule.GetLevelFlowController(
                    GetCommandEvaluationService(),
                    _lazyUnitTurnOrderer.Value,
                    _lazyPlayerDecisionService.Value,
                    GetSequencePlayer()
                )
            );
        }

        public LevelData GetLevelData() => _levelModule.LevelData;

        public ILevelFlowController GetLevelFlowController() => _lazyLevelFlowController.Value;
        public ICommandEvaluationService GetCommandEvaluationService() => _lazyCommandEvaluationService.Value;
        public ISequencePlayer GetSequencePlayer() => _lazySequencePlayer.Value;
    }
}