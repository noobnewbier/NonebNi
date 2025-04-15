using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using NonebNi.Core.Sequences;
using NonebNi.Terrain;
using NonebNi.Ui.Animation;
using NonebNi.Ui.Animation.Sequence;
using NonebNi.Ui.Entities;
using Unity.Logging;

namespace NonebNi.Ui.Sequences
{
    public class SequencePlayer : ISequencePlayer
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        private readonly IEntityRepository _entityRepository;

        public SequencePlayer(
            IEntityRepository entityRepository,
            ICoordinateAndPositionService coordinateAndPositionService)
        {
            _entityRepository = entityRepository;
            _coordinateAndPositionService = coordinateAndPositionService;
        }

        public async UniTask Play(IEnumerable<ISequence> sequences)
        {
            foreach (var sequence in sequences) await Play(sequence);
        }

        private async UniTask Play(ISequence sequence)
        {
            async UniTask PlaySequence()
            {
                switch (sequence)
                {
                    case AggregateSequence aggregateSequence:
                    {
                        var routines = aggregateSequence.Sequences.Select(Play).ToArray();
                        foreach (var routine in routines) await routine;
                        break;
                    }

                    case DieSequence dieSequence:
                    {
                        var entity = _entityRepository.GetEntity(dieSequence.DeadUnit.Guid);
                        if (entity != null)
                            await PlayAnimation(entity, new DieAnimSequence());

                        break;
                    }

                    case TeleportSequence teleportSequence:
                    {
                        var entity = _entityRepository.GetEntity(teleportSequence.Unit.Guid);
                        if (entity != null)
                            await PlayAnimation(
                                entity,
                                new TeleportAnimSequence(
                                    _coordinateAndPositionService.FindPosition(teleportSequence.TargetPos)
                                )
                            );

                        break;
                    }

                    case MoveSequence moveSequence:
                    {
                        var entity = _entityRepository.GetEntity(moveSequence.MovedEntity.Guid);
                        if (entity != null)
                            await PlayAnimation(
                                entity,
                                new MoveAnimSequence(
                                    moveSequence.TargetCoords.Select(c => _coordinateAndPositionService.FindPosition(c))
                                )
                            );

                        break;
                    }

                    case KnockBackSequence knockBackSequence:
                    {
                        var entity = _entityRepository.GetEntity(knockBackSequence.MovedUnit.Guid);
                        if (entity != null)
                            await PlayAnimation(
                                entity,
                                new KnockBackAnimSequence(_coordinateAndPositionService.FindPosition(knockBackSequence.TargetCoord))
                            );

                        break;
                    }

                    case DamageSequence damageSequence:
                    {
                        var receiver = _entityRepository.GetEntity(damageSequence.DamageReceiver.Guid);
                        if (receiver != null)
                            await PlayAnimation(receiver, new ReceivedDamageAnimSequence());

                        var actor = _entityRepository.GetEntity(damageSequence.ActionCaster.Guid);
                        if (actor != null)
                            await PlayAnimation(
                                actor,
                                new ApplyDamageAnimSequence(damageSequence.AnimId, receiver)
                            );

                        break;
                    }
                }
            }

            await PlaySequence();
        }

        private UniTask PlayAnimation<T>(Entity entity, T sequence) where T : IAnimSequence
        {
            var animControl = entity.GetAnimationControl<T>();
            if (animControl == null)
            {
                Log.Error($"Failed to find component of type  {nameof(IPlayAnimation)}<{typeof(T).Namespace}>");
                return UniTask.CompletedTask;
            }

            return animControl.Play(sequence);
        }
    }
}