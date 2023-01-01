using System.Collections;
using NonebNi.Ui.Animation.Sequence;
using NonebNi.Ui.Common;
using NonebNi.Ui.Common.Attributes;
using UnityEngine;

namespace NonebNi.Ui.Animation
{
    public class PrototypeAnimationControl : MonoBehaviour, IPlayAnimation<DieAnimSequence>,
        IPlayAnimation<TeleportAnimSequence>
    {
        [SerializeField] private Animator animator = null!;

        [AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)] [SerializeField]
        private string triggerName = null!;

        [AnimatorState(nameof(animator), nameof(finishAnimLayerIndex))] [SerializeField]
        private string finishAnimState = null!;

        [AnimatorLayer(nameof(animator))] [SerializeField]
        private int finishAnimLayerIndex;

        [ContextMenu("Die")]
        public Coroutine Play(DieAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                animator.SetTrigger(triggerName);

                yield return new WaitForAnimatorState(animator, finishAnimLayerIndex, finishAnimState);
            }

            return StartCoroutine(Coroutine());
        }

        public Coroutine Play(TeleportAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                transform.position = sequence.TargetTilePosition;

                yield break;
            }

            return StartCoroutine(Coroutine());
        }
    }
}