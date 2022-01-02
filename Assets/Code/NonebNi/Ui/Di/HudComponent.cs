using NonebNi.Core.Di;
using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;

namespace NonebNi.Ui.Di
{
    public interface IHudComponent
    {
        public LevelData GetLevelData();
        public ILevelFlowController GetLevelFlowController();
    }

    public class HudComponent : IHudComponent
    {
        private readonly ILevelComponent _levelComponent;

        public HudComponent(ILevelComponent levelComponent)
        {
            _levelComponent = levelComponent;
        }

        public LevelData GetLevelData() => _levelComponent.GetLevelData();
        public ILevelFlowController GetLevelFlowController() => _levelComponent.GetLevelFlowController();
    }
}