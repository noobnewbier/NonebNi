using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Sequences
{
    public class TeleportSequence : ISequence
    {
        public readonly Coordinate TargetPos;
        public readonly UnitData Unit;

        public TeleportSequence(UnitData unit, Coordinate targetPos)
        {
            Unit = unit;
            TargetPos = targetPos;
        }
    }
}