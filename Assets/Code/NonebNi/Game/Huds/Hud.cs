using NonebNi.Game.Di;
using UnityEngine;

namespace NonebNi.Game.Huds
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private LevelNameView levelNameView;

        private IHudComponent _hudComponent = null!;

        public void Init(GameModule gameModule)
        {
            _hudComponent = new HudComponent(gameModule);

            levelNameView.Init(_hudComponent);
        }
    }
}