using NonebNi.Core.Entities;

namespace NonebNi.Core.Actions
{
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