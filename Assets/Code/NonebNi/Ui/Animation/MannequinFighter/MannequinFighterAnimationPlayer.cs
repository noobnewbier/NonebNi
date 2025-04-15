using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Noneb.UI.Animation;
using NonebNi.Ui.Animation.Common;
using NonebNi.Ui.Animation.Sequence;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.Animation.MannequinFighter
{
    //todo: bug - why is it taking so long to the next turn
    //todo: a way to show enemy health
    //TODO: current goal - get your walk animation "working"
    //TODO: next goal - get slash animation "working", you probs want to mix in IK for concept.
    //todo: with this pattern we might be able to get away with just one anim player.
    //nothing needs to be polished, just to a point that it works 
    public class MannequinFighterAnimationPlayer :
        MonoBehaviour,
        IPlayAnimation<DieAnimSequence>,
        IPlayAnimation<TeleportAnimSequence>,
        IPlayAnimation<MoveAnimSequence>,
        IPlayAnimation<KnockBackAnimSequence>,
        IPlayAnimation<ReceivedDamageAnimSequence>,
        IPlayAnimation<ApplyDamageAnimSequence>
    {
        [SerializeField] private Animator animator = null!;
        [SerializeField] private PrototypeMeleeWeaponAnimationControl meleeWeaponAnimControl = null!;
        [SerializeField] private MovementAnimationControl movementControl = null!;
        [SerializeField] private AnimationDataTable animTable = new();
        [SerializeField] private AnimationData fallbackAnim = new();


        [ContextMenu(nameof(Play))]
        public async UniTask Play(ApplyDamageAnimSequence sequence, CancellationToken ct = default)
        {
            var animTask = PlayAnimation(sequence.AnimId, ct);
            var waitForHitTask = sequence.DamageReceiver == null ?
                UniTask.CompletedTask :
                meleeWeaponAnimControl.WaitTillHitEntity(sequence.DamageReceiver, ct);

            await UniTask.WhenAny(
                animTask,
                waitForHitTask
            );
        }

        public async UniTask Play(DieAnimSequence sequence, CancellationToken ct = default)
        {
            await PlayAnimation("die", ct);
        }

        public UniTask Play(KnockBackAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        public async UniTask Play(MoveAnimSequence sequence, CancellationToken ct = default)
        {
            await movementControl.WalkTo(sequence.TargetPositions, ct);
        }

        public async UniTask Play(ReceivedDamageAnimSequence sequence, CancellationToken ct = default)
        {
            await PlayAnimation("take-damage", ct);
        }

        public UniTask Play(TeleportAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        private async UniTask PlayAnimation(string animId, CancellationToken ct = default)
        {
            var data = animTable.FindAnim(animId);
            if (data == null)
            {
                Log.Error($@"Cannot find animation with Id ""{animId}"", using fallback animation");
                data = fallbackAnim;
            }

            await UniTask.WhenAny(
                UniTask.WaitForSeconds(10, cancellationToken: ct),
                animator.PlayAnimation(data, ct)
            );
        }
    }
}