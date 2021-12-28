using NonebNi.Core.Level;

namespace NonebNi.Ui.Di
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