using NonebNi.Core.Entities;

namespace NonebNi.Core.Actions
{
    public abstract class Range
    {
        public abstract int CalculateRange(EntityData actionCaster);

        public static implicit operator Range(int range) => new ConstantRange(range);
    }

    public class ConstantRange : Range
    {
        private readonly int _range;

        public ConstantRange(int range)
        {
            _range = range;
        }

        public override int CalculateRange(EntityData _) => _range;
    }
}