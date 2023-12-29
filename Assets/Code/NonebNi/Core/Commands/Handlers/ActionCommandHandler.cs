using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Commands.Handlers
{
    public class ActionCommandHandler : ICommandHandler<ActionCommand>
    {
        private readonly IMap _map;
        private readonly ITargetFinder _targetFinder;

        public ActionCommandHandler(IMap map, ITargetFinder targetFinder)
        {
            _map = map;
            _targetFinder = targetFinder;
        }

        public IEnumerable<ISequence> Evaluate(ActionCommand command)
        {
            return command.Action.Effects.SelectMany(
                e =>
                {
                    var targets = _targetFinder.FindTargets(
                        command.ActorEntity,
                        command.TargetCoords,
                        command.Action.TargetArea,
                        command.Action.TargetRestrictions
                    );
                    return e.Evaluate(_map, command.ActorEntity, targets);
                }
            );
        }
    }
}