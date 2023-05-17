using System.Collections.Generic;
using NonebNi.Core.Coordinates;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Commands.Handlers
{
    public class TeleportCommandHandler : ICommandHandler<TeleportCommand>
    {
        private readonly IMap _map;

        public TeleportCommandHandler(IMap map)
        {
            _map = map;
        }

        public IEnumerable<ISequence> Evaluate(TeleportCommand command)
        {
            if (!_map.TryFind(command.UnitData, out Coordinate _)) yield break;

            var moveResult = _map.Move(command.UnitData, command.TargetPos);
            if (moveResult == MoveResult.Success)
                yield return new TeleportSequence(command.UnitData, command.TargetPos);
        }
    }
}