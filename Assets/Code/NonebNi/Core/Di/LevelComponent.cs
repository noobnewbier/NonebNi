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
        private readonly Lazy<ILevelFlowController> _lazyLevelFlowController;
        private readonly LevelModule _levelModule;

        public LevelComponent(LevelModule levelModule)
        {
            _levelModule = levelModule;
            _lazyLevelFlowController = new Lazy<ILevelFlowController>(levelModule.GetLevelFlowController);
        }

        public LevelData GetLevelData() => _levelModule.LevelData;

        public ILevelFlowController GetLevelFlowController() => _lazyLevelFlowController.Value;
    }
}