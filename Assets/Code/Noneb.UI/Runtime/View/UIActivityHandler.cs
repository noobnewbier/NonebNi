using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Noneb.UI.View
{
    public class UIActivityHandler : MonoBehaviour, IViewComponent<NullViewData>
    {
        [SerializeField] private CanvasGroup canvasGroup = null!;

        private bool _canBlockRaycast;

        public UniTask OnViewAwake()
        {
            _canBlockRaycast = canvasGroup.blocksRaycasts;
            return UniTask.CompletedTask;
        }

        public UniTask OnViewInit()
        {
            gameObject.SetActive(true);
            return UniTask.CompletedTask;
        }

        //TODO: probs need overlay check...
        public UniTask OnViewActivate(NullViewData? _)
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

        public UniTask OnViewTearDown()
        {
            gameObject.SetActive(false);
            return UniTask.CompletedTask;
        }
    }
}