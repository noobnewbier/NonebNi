using NonebNi.Core.Entities;
using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class DamageSequence : ISequence
    {
        private readonly EntityData _actionCaster;
        private readonly int _damage;
        public readonly UnitData DamageReceiver;

        public DamageSequence(EntityData actionCaster, UnitData damageReceiver, int damage)
        {
            _actionCaster = actionCaster;
            DamageReceiver = damageReceiver;
            _damage = damage;
        }
    }
}