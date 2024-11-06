using System.Collections;
using Cysharp.Threading.Tasks;
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
        IPlayAnimation<MoveAnimSequence>,
        IPlayAnimation<KnockBackAnimSequence>,
        IPlayAnimation<ReceivedDamageAnimSequence>
    {
        [SerializeField] private Animator animator = null!;

        [AnimatorParameter(nameof(animator), AnimatorControllerParameterType.Trigger)] [SerializeField]
        private string triggerName = null!;

        [AnimatorState(nameof(animator), nameof(finishAnimLayerIndex))] [SerializeField]
        private string finishAnimState = null!;

        [AnimatorLayer(nameof(animator))] [SerializeField]
        private int finishAnimLayerIndex;

        public UniTask Play(DieAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                animator.SetTrigger(triggerName);

                yield return new WaitForAnimatorState(animator, finishAnimLayerIndex, finishAnimState);
            }

            return Coroutine().ToUniTask(this);
        }

        public UniTask Play(KnockBackAnimSequence sequence)
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

            return Coroutine().ToUniTask(this);
        }

        public UniTask Play(MoveAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                const float epsilon = 0.01f;
                while (Vector3.Distance(transform.position, sequence.TargetPos) > epsilon)
                {
                    transform.position = Vector3.MoveTowards(transform.position, sequence.TargetPos, 0.25f);
                    yield return null;
                }
            }

            return Coroutine().ToUniTask(this);
        }

        public UniTask Play(ReceivedDamageAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                //just a quick jump for now, we can improve this later:
                var originalPos = transform.position;
                var peakPos = originalPos + Vector3.up;

                while (transform.position.y < peakPos.y)
                {
                    var newPos = Vector3.MoveTowards(transform.position, peakPos, 2f * Time.deltaTime);
                    transform.position = newPos;

                    yield return null;
                }

                while (transform.position.y > originalPos.y)
                {
                    var newPos = Vector3.MoveTowards(transform.position, originalPos, 2f * Time.deltaTime);
                    transform.position = newPos;

                    yield return null;
                }
            }

            return Coroutine().ToUniTask(this);
        }

        public UniTask Play(TeleportAnimSequence sequence)
        {
            IEnumerator Coroutine()
            {
                transform.position = sequence.TargetTilePosition;

                yield break;
            }

            return Coroutine().ToUniTask(this);
        }
    }
}