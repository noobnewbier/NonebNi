using NonebNi.Core.Entities;

namespace NonebNi.Core.Effects
{
    public abstract class Damage
    {
        private readonly EntityData actor;

        public abstract int CalculateDamage(EntityData target);

        public static implicit operator Damage(int amount) => new ConstantDamage(amount);
    }
}