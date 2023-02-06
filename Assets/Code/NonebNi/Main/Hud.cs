using NonebNi.Main.Di;
using NonebNi.Ui.Huds;
using UnityEngine;
using UnityEngine.Serialization;

namespace NonebNi.Main
{
    public class Hud : MonoBehaviour
    {
        [FormerlySerializedAs("levelNameView")] [SerializeField]
        private PrototypeView prototypeView = null!;

        private IHudComponent _hudComponent = null!;

        public void Init(IHudComponent hudComponent)
        {
            _hudComponent = hudComponent;

            prototypeView.Init(_hudComponent.GetLevelData(), _hudComponent.GetAgentDecisionService());
        }
    }
}