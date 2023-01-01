using System;
using System.Collections.Generic;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Commands.Handlers
{
    public class DamageCommandHandler : ICommandHandler<DamageCommand>
    {
        private readonly IMap _map;

        public DamageCommandHandler(IMap map)
        {
            _map = map;
        }

        public IEnumerable<ISequence> Evaluate(DamageCommand command)
        {
            foreach (var target in command.Targets) target.Health -= command.Damage;

            foreach (var unit in command.Targets)
                if (unit.Health <= 0)
                {
                    if (!_map.Remove(unit))
                        throw new InvalidOperationException(
                            "Shouldn't be able to evaluate command with targets that's ain't even on the map"
                        );

                    yield return new DieSequence(unit);
                }
        }
    }
}