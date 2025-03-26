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

        public async UniTask OnViewEnter(INonebView? previousView)
        {
            await uiAnimator.PlayAnimation(enterAnimationData);
        }

        public async UniTask OnViewLeave(INonebView? nextView)
        {
            await uiAnimator.PlayAnimation(leaveAnimationData);
        }
    }
}