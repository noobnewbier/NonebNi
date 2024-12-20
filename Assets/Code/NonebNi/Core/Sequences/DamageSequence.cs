using NonebNi.Core.Entities;
using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class DamageSequence : ISequence
    {
        private readonly EntityData _actionCaster;

        private readonly int _damage;

        //TODO: anim id really needs a data ref like picker.
        public readonly string AnimId;
        public readonly UnitData DamageReceiver;

        public DamageSequence(EntityData actionCaster, UnitData damageReceiver, int damage, string animId)
        {
            _actionCaster = actionCaster;
            DamageReceiver = damageReceiver;
            _damage = damage;
            AnimId = animId;
        }
    }
}