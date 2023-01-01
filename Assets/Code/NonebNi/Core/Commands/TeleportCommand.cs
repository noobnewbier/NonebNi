using NonebNi.Core.Coordinates;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands
{
    /// <summary>
    ///     Teleporting a unit to designated position. Regardless if there's a valid path between start and end position.
    ///     Expect to fail if there's already something in the target position, teleporting to the current position should have no
    ///     effect
    /// </summary>
    public class TeleportCommand : ICommand
    {
        public readonly Coordinate TargetPos;
        public readonly UnitData UnitData;

        public TeleportCommand(UnitData unitData, Coordinate targetPos)
        {
            TargetPos = targetPos;
            UnitData = unitData;
        }
    }
}