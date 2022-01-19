using NonebNi.Core.Sequences;
using NonebNi.Ui.Entities;
using UnityEngine;

namespace NonebNi.Ui.Sequences
{
    public class SequencePlayer : ISequencePlayer
    {
        private readonly IEntityRepository _entityRepository;

        public SequencePlayer(IEntityRepository entityRepository)
        {
            _entityRepository = entityRepository;
        }

        public Coroutine? Play(ISequence sequence)
        {
            if (sequence is DieSequence dieSequence)
            {
                var entity = _entityRepository.GetEntity(dieSequence.DeadUnit.Guid);
                if (entity != null) return entity.GetAnimationControl<PrototypeDieAnimationControl>().Play(new Context());
            }

            return null;
        }
    }
}