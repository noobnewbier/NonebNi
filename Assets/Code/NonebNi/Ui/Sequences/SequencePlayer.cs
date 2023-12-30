using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Sequences;
using NonebNi.Terrain;
using NonebNi.Ui.Animation;
using NonebNi.Ui.Animation.Sequence;
using NonebNi.Ui.Entities;
using UnityEngine;

namespace NonebNi.Ui.Sequences
{
    public class SequencePlayer : ISequencePlayer
    {
        private readonly ICoordinateAndPositionService _coordinateAndPositionService;

        //Deliberately not creating a static API for it - we might want to migrate to UniTask in the future which may resolve the issue without doing this?
        //By not having an external class we keep the whole mess in one place which is better than scattering it through multiple files
        private readonly CoroutineRunner _coroutineRunner;

        private readonly IEntityRepository _entityRepository;

        public SequencePlayer(
            IEntityRepository entityRepository,
            ICoordinateAndPositionService coordinateAndPositionService)
        {
            _entityRepository = entityRepository;
            _coordinateAndPositionService = coordinateAndPositionService;

            var coroutineRunnerObject = new GameObject("SequencePlayerCoroutineRunner");
            Object.DontDestroyOnLoad(coroutineRunnerObject);

            _coroutineRunner = coroutineRunnerObject.AddComponent<CoroutineRunner>();
        }

        public IEnumerator Play(IEnumerable<ISequence> sequences)
        {
            foreach (var sequence in sequences) yield return Play(sequence);
        }

        private Coroutine Play(ISequence sequence)
        {
            IEnumerator PlaySequence()
            {
                switch (sequence)
                {
                    case AggregateSequence aggregateSequence:
                    {
                        var routines = aggregateSequence.Sequences.Select(Play).ToArray();
                        foreach (var routine in routines) yield return routine;
                        break;
                    }

                    case DieSequence dieSequence:
                    {
                        var entity = _entityRepository.GetEntity(dieSequence.DeadUnit.Guid);
                        if (entity != null)
                            yield return entity.GetAnimationControl<IPlayAnimation<DieAnimSequence>>()
                                .Play(new DieAnimSequence());

                        break;
                    }

                    case TeleportSequence teleportSequence:
                    {
                        var entity = _entityRepository.GetEntity(teleportSequence.Unit.Guid);
                        if (entity != null)
                            yield return entity.GetAnimationControl<IPlayAnimation<TeleportAnimSequence>>().Play(
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
                            yield return entity.GetAnimationControl<IPlayAnimation<MoveAnimSequence>>()
                                .Play(
                                    new MoveAnimSequence(
                                        _coordinateAndPositionService.FindPosition(moveSequence.TargetCoord)
                                    )
                                );

                        break;
                    }

                    case KnockBackSequence knockBackSequence:
                    {
                        var entity = _entityRepository.GetEntity(knockBackSequence.MovedUnit.Guid);
                        if (entity != null)
                            yield return entity.GetAnimationControl<IPlayAnimation<KnockBackAnimSequence>>()
                                .Play(
                                    new KnockBackAnimSequence(
                                        _coordinateAndPositionService.FindPosition(knockBackSequence.TargetCoord)
                                    )
                                );

                        break;
                    }

                    case DamageSequence damageSequence:
                    {
                        var entity = _entityRepository.GetEntity(damageSequence.DamageReceiver.Guid);
                        if (entity != null)
                            yield return entity.GetAnimationControl<IPlayAnimation<DamageAnimSequence>>()
                                .Play(
                                    new DamageAnimSequence()
                                );

                        break;
                    }
                }
            }

            return _coroutineRunner.StartCoroutine(PlaySequence());
        }

        /// <summary>
        ///     Deliberately empty - only purpose is that <see cref="SequencePlayer" /> can run a coroutine
        /// </summary>
        private class CoroutineRunner : MonoBehaviour { }
    }
}