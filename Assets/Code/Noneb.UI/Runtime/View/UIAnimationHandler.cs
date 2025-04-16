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

        public async UniTask OnViewEnter(INonebView? previousView, INonebView currentView)
        {
            //todo: who is my view wtf mate
            if (previousView == currentView)
                // no need to replay the enter animation
                return;

            await uiAnimator.PlayAnimation(enterAnimationData);
        }

        public async UniTask OnViewLeave(INonebView currentView, INonebView? nextView)
        {
            if (currentView == nextView)
                // no need to play the leave animation - we are coming back
                return;

            await uiAnimator.PlayAnimation(leaveAnimationData);
        }
    }
}