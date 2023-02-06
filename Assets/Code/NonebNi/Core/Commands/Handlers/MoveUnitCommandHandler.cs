using System.Collections.Generic;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;
using NonebNi.Core.Units;

namespace NonebNi.Core.Commands.Handlers
{
    public class MoveUnitCommandHandler : ICommandHandler<MoveUnitCommand>
    {
        private readonly IMap _map;

        public MoveUnitCommandHandler(IMap map)
        {
            _map = map;
        }

        public IEnumerable<ISequence> Evaluate(MoveUnitCommand unitCommand)
        {
            var result = _map.Move(unitCommand.MovedUnit, unitCommand.EndCoord);

            if (result == MoveResult.Success)
            {
                yield return new MoveSequence(unitCommand.MovedUnit, unitCommand.EndCoord);
            }
        }
    }
}