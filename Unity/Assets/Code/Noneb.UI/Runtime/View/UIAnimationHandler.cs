using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using UnityEngine;

namespace Noneb.UI.View
{
    public class UIAnimationHandler : MonoBehaviour, IViewComponent
    {
        [SerializeField] private Animator uiAnimator = null!;
        [SerializeField] private AnimationData enterAnimationData = null!;
        [SerializeField] private AnimationData leaveAnimationData = null!;

        [SerializeField] private bool shouldAnimOnTransitionSelfTransition = true;

        public async UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            if (previousView == currentView && !shouldAnimOnTransitionSelfTransition)
                // no need to replay the enter animation
                return;

            await uiAnimator.PlayAnimation(enterAnimationData);
        }

        public async UniTask OnViewLeave(INonebView currentView, INonebView? nextView)
        {
            if (currentView == nextView && !shouldAnimOnTransitionSelfTransition)
                // no need to play the leave animation - we are coming back
                return;

            await uiAnimator.PlayAnimation(leaveAnimationData);
        }
    }
}