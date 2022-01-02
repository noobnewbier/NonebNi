using NonebNi.Ui.Di;
using UnityEngine;
using UnityEngine.Serialization;

namespace NonebNi.Ui.Huds
{
    public class Hud : MonoBehaviour
    {
        [FormerlySerializedAs("levelNameView")] [SerializeField]
        private PrototypeView prototypeView;

        private IHudComponent _hudComponent = null!;

        public void Init(IHudComponent hudComponent)
        {
            _hudComponent = hudComponent;

            prototypeView.Init(_hudComponent);
        }
    }
}