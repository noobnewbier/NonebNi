using System.Collections;
using System.Collections.Generic;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Sequences;
using NonebNi.Ui.Animation;
using NonebNi.Ui.Animation.Sequence;
using NonebNi.Ui.Entities;
using UnityEngine;

namespace NonebNi.Ui.Sequences
{
    public class SequencePlayer : ISequencePlayer
    {
        private readonly CoordinateAndPositionService _coordinateAndPositionService;

        //Deliberately not creating a static API for it - we might want to migrate to UniTask in the future which may resolve the issue without doing this?
        //By not having an external class we keep the whole mess in one place which is better than scattering it through multiple files
        private readonly CoroutineRunner _coroutineRunner;

        private readonly IEntityRepository _entityRepository;

        public SequencePlayer(IEntityRepository entityRepository, CoordinateAndPositionService coordinateAndPositionService)
        {
            _entityRepository = entityRepository;
            _coordinateAndPositionService = coordinateAndPositionService;

            var coroutineRunnerObject = new GameObject("SequencePlayerCoroutineRunner");
            Object.DontDestroyOnLoad(coroutineRunnerObject);

            _coroutineRunner = coroutineRunnerObject.AddComponent<CoroutineRunner>();
        }

        public Coroutine Play(IEnumerable<ISequence> sequences)
        {
            IEnumerator PlaySequences()
            {
                foreach (var sequence in sequences)
                    switch (sequence)
                    {
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
                                        _coordinateAndPositionService.FindPosition(teleportSequence.TargetPos))
                                );

                            break;
                        }
                    }
            }

            return _coroutineRunner.StartCoroutine(PlaySequences());
        }

        /// <summary>
        ///     Deliberately empty - only purpose is that <see cref="SequencePlayer" /> can run a coroutine
        /// </summary>
        private class CoroutineRunner : MonoBehaviour { }
    }
}