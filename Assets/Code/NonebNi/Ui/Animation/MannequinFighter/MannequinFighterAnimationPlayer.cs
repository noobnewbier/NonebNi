using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NonebNi.Ui.Animation.Common;
using NonebNi.Ui.Animation.Sequence;
using Unity.Logging;
using UnityEngine;

namespace NonebNi.Ui.Animation.MannequinFighter
{
    //TODO: current goal - get your walk animation "working"
    //TODO: next goal - get slash animation "working", you probs want to mix in IK for concept.
    //nothing needs to be polished, just to a point that it works 
    public class MannequinFighterAnimationPlayer :
        MonoBehaviour,
        IPlayAnimation<DieAnimSequence>,
        IPlayAnimation<TeleportAnimSequence>,
        IPlayAnimation<MoveAnimSequence>,
        IPlayAnimation<KnockBackAnimSequence>,
        IPlayAnimation<ReceivedDamageAnimSequence>
    {
        [SerializeField] private Animator animator = null!;
        [SerializeField] private PrototypeMeleeWeaponAnimationControl meleeWeaponAnimControl = null!;
        [SerializeField] private MovementAnimationControl movementControl = null!;
        [SerializeField] private AnimationDataTable animaTable = new();

        public UniTask Play(DieAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        public UniTask Play(KnockBackAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        public async UniTask Play(MoveAnimSequence sequence, CancellationToken ct = default)
        {
            await movementControl.WalkTo(sequence.TargetPositions, ct);
        }

        public UniTask Play(ReceivedDamageAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        public UniTask Play(TeleportAnimSequence sequence, CancellationToken ct = default) => throw new NotImplementedException();

        [ContextMenu(nameof(Play))]
        public async UniTask Play(ApplyDamageAnimSequence sequence)
        {
            //TODO: should probably work with ct 
            var animTask = PlayAnimation(sequence.AnimId);
            var waitForHitTask = sequence.DamageReceiver == null ?
                UniTask.CompletedTask :
                meleeWeaponAnimControl.WaitTillHitEntity(sequence.DamageReceiver);

            await UniTask.WhenAny(
                animTask,
                waitForHitTask
            );
        }

        private async UniTask PlayAnimation(string animId)
        {
            var data = animaTable.FindAnim(animId);
            if (data == null)
            {
                Log.Error($@"Cannot find animation with Id ""{animId}""");
                return;
            }

            await animator.PlayAnimation(data);
        }
    }
}