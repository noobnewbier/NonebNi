using NonebNi.Ui.Di;
using UnityEngine;

namespace NonebNi.Ui.Huds
{
    public class Hud : MonoBehaviour
    {
        [SerializeField] private LevelNameView levelNameView;

        private IHudComponent _hudComponent = null!;

        public void Init(IHudComponent hudComponent)
        {
            _hudComponent = hudComponent;

            levelNameView.Init(_hudComponent);
        }
    }
}