using System.Collections;
using NonebNi.Ui.Animation.Sequence;
using NonebNi.Ui.Common;
using NonebNi.Ui.Common.Attributes;
using UnityEngine;

namespace NonebNi.Ui.Animation
{
    public class PrototypeAnimationControl : 
        MonoBehaviour, 
        IPlayAnimation<DieAnimSequence>,
        IPlayAnimation<TeleportAnimSequence>,
        IPlayAnimation<MoveAnimSequence>
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

        public Coroutine Play(MoveAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                const float epsilon = 0.01f;
                while (Vector3.Distance(transform.position, sequence.TargetPos) > epsilon)
                {
                    transform.position = Vector3.MoveTowards(transform.position, sequence.TargetPos, 1f);
                    yield return null;
                }
            }

            return StartCoroutine(Coroutine());
        }
    }
}