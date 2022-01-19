using System.Collections;
using NonebNi.Ui.Common;
using NonebNi.Ui.Common.Attributes;
using NonebNi.Ui.Entities;
using UnityEngine;

namespace NonebNi.Ui
{
    public class PrototypeDieAnimationControl : MonoBehaviour, IAnimationControl
    {
        [SerializeField] private Animator animator = null!;

        [AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)] [SerializeField]
        private string triggerName = null!;

        [AnimatorState(nameof(animator), nameof(finishAnimLayerIndex))] [SerializeField]
        private string finishAnimState = null!;

        [AnimatorLayer(nameof(animator))] [SerializeField]
        private int finishAnimLayerIndex;


        [ContextMenu("Play")]
        public Coroutine Play(Context context)
        {
            IEnumerator Coroutine()
            {
                animator.SetTrigger(triggerName);

                yield return new WaitForAnimatorState(animator, finishAnimLayerIndex, finishAnimState);
            }

            return StartCoroutine(Coroutine());
        }
    }
}