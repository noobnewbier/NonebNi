using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Noneb.UI.View
{
    public class UIActivityHandler : MonoBehaviour, IViewComponent
    {
        [SerializeField] private CanvasGroup canvasGroup = null!;

        private bool _canBlockRaycast;

        public UniTask OnViewInit()
        {
            _canBlockRaycast = canvasGroup.blocksRaycasts;
            return UniTask.CompletedTask;
        }

        //TODO: probs need overlay check...
        public UniTask OnViewActivate()
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = _canBlockRaycast;

            return UniTask.CompletedTask;
        }

        public UniTask OnViewDeactivate()
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            return UniTask.CompletedTask;
        }
    }
}