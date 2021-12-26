using NonebNi.Game.Level;

namespace NonebNi.Game.Di
{
    public interface IHudComponent
    {
        public LevelData GetLevelData();
    }

    public class HudComponent : IHudComponent
    {
        private readonly GameModule _module;

        public HudComponent(GameModule module)
        {
            _module = module;
        }

        public LevelData GetLevelData() => _module.LevelData;
    }
}