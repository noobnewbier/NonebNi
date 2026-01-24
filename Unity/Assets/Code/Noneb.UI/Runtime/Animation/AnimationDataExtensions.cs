using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Noneb.UI.Animation
{
    public static class AnimationDataExtensions
    {
        public static async UniTask PlayAnimation(this Animator animator, AnimationData data, CancellationToken ct = default)
        {
            //TODO: future me - honest to god, I don't know if this works :)
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, animator.gameObject.GetCancellationTokenOnDestroy());
            var wasApplyingRootMotion = animator.applyRootMotion;
            animator.applyRootMotion = data.IsRootMotion;

            switch (data.GetParameterType())
            {
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(data.Name, data.TargetNumericValue);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(data.Name, (int)data.TargetNumericValue);
                    break;
                case AnimatorControllerParameterType.Bool:
                    if (animator.GetBool(data.Name) != data.TargetBoolValue) animator.SetBool(data.Name, data.TargetBoolValue);

                    await new WaitForAnimatorState(animator, data.FinishAnimLayerIndex, data.FinishAnimState).WithCancellation(linkedCts.Token);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(data.Name);
                    await new WaitForAnimatorState(animator, data.FinishAnimLayerIndex, data.FinishAnimState).WithCancellation(linkedCts.Token);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            animator.applyRootMotion = wasApplyingRootMotion;
        }
    }
}