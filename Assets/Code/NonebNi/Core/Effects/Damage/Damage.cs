using NonebNi.Core.Entities;

namespace NonebNi.Core.Effects
{
    public abstract class Damage
    {
        public abstract int CalculateDamage(EntityData actionCaster, EntityData target);

        public static implicit operator Damage(int amount) => new ConstantDamage(amount);
    }
}