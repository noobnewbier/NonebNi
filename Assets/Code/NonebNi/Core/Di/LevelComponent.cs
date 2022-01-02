using System;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;

namespace NonebNi.Core.Di
{
    public interface ILevelComponent
    {
        LevelData GetLevelData();
        ILevelFlowController GetLevelFlowController();
    }

    public class LevelComponent : ILevelComponent
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Lazy<ICommandEvaluationService> _lazyCommandEvaluationService;
        private readonly Lazy<ILevelFlowController> _lazyLevelFlowController;
        private readonly LevelModule _levelModule;

        public LevelComponent(LevelModule levelModule, CommandEvaluationModule commandEvaluationModule)
        {
            _levelModule = levelModule;
            _lazyCommandEvaluationService =
                new Lazy<ICommandEvaluationService>(commandEvaluationModule.GetCommandEvaluationService);
            _lazyLevelFlowController = new Lazy<ILevelFlowController>(
                () => levelModule.GetLevelFlowController(_lazyCommandEvaluationService.Value)
            );
        }

        public LevelData GetLevelData() => _levelModule.LevelData;

        public ILevelFlowController GetLevelFlowController() => _lazyLevelFlowController.Value;
    }
}