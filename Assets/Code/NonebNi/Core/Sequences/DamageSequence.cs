using NonebNi.Core.Entities;
using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class DamageSequence : ISequence
    {
        private readonly int _damage;

        //TODO: anim id really needs a data ref like picker.
        public readonly EntityData ActionCaster;
        public readonly string AnimId;
        public readonly UnitData DamageReceiver;

        public DamageSequence(EntityData actionCaster, UnitData damageReceiver, int damage, string animId)
        {
            ActionCaster = actionCaster;
            DamageReceiver = damageReceiver;
            _damage = damage;
            AnimId = animId;
        }
    }
}