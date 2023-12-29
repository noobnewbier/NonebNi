using NonebNi.Core.Entities;

namespace NonebNi.Core.Actions
{
    public abstract class Range
    {
        public abstract int CalculateRange(EntityData actionCaster);

        public static implicit operator Range(int range) => new ConstantRange(range);
    }
}