using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class DieSequence : ISequence
    {
        public readonly UnitData DeadUnit;

        public DieSequence(UnitData deadUnit)
        {
            DeadUnit = deadUnit;
        }
    }
}