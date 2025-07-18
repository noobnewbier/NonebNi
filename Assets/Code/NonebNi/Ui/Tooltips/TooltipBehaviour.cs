using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.Tooltips
{
    /// <summary>
    /// Generic shenanigans for wildcards
    /// </summary>
    public abstract class TooltipBehaviour : MonoBehaviour
    {
        [SerializeField] private Animator uiAnimator = null!;
        [SerializeField] private AnimationData showAnimationData = null!;
        [SerializeField] private AnimationData hideAnimationData = null!;

        private bool _isVisible;

        public async UniTask Hide()
        {
            if (!_isVisible) return;

            _isVisible = false;
            await uiAnimator.PlayAnimation(hideAnimationData);
        }

        public async UniTask Show(TooltipRequest request)
        {
            var populateTask = Populate(request);
            if (_isVisible) return;

            _isVisible = true;
            await populateTask;
            await uiAnimator.PlayAnimation(showAnimationData);
        }

        protected abstract UniTask Populate(TooltipRequest request);
    }

    public abstract class TooltipBehaviour<TRequest> : TooltipBehaviour where TRequest : TooltipRequest
    {
        protected override async UniTask Populate(TooltipRequest request)
        {
            if (request is not TRequest typedRequest)
            {
                Log.Error($"Failed to populate tooltip request - expected {typeof(TRequest).Name} but got {request.GetType().Name}");
                return;
            }

            await OnPopulate(typedRequest);
        }

        protected abstract UniTask OnPopulate(TRequest request);
    }
}