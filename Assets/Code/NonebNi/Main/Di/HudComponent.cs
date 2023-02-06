using NonebNi.Core.Agents;
using NonebNi.Core.Level;

namespace NonebNi.Main.Di
{
    public interface IHudComponent
    {
        public LevelData GetLevelData();
        public IPlayerAgent GetPlayerAgent();
    }

    public class HudComponent : IHudComponent
    {
        private readonly ILevelComponent _levelComponent;

        public HudComponent(ILevelComponent levelComponent)
        {
            _levelComponent = levelComponent;
        }

        public LevelData GetLevelData() => _levelComponent.GetLevelData();
        public IPlayerAgent GetPlayerAgent() => _levelComponent.GetPlayerAgent();
    }
}