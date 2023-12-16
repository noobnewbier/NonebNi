using NonebNi.Core.Entities;

namespace NonebNi.Core.Effects
{
    public class ConstantDamage : Damage
    {
        private readonly int _amount;

        public ConstantDamage(int amount)
        {
            _amount = amount;
        }

        public override int CalculateDamage(EntityData _) => _amount;
    }
}