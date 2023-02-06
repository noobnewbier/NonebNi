using NonebNi.Core.FlowControl;
using NonebNi.Core.Level;

namespace NonebNi.Main.Di
{
    public interface IHudComponent
    {
        public LevelData GetLevelData();
        public IAgentDecisionService GetAgentDecisionService();
    }

    public class HudComponent : IHudComponent
    {
        private readonly ILevelComponent _levelComponent;

        public HudComponent(ILevelComponent levelComponent)
        {
            _levelComponent = levelComponent;
        }

        public LevelData GetLevelData() => _levelComponent.GetLevelData();
        public IAgentDecisionService GetAgentDecisionService() => _levelComponent.GetAgentDecisionService();

    }
}