using System;
using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.FlowControl
{
    public interface ICommandEvaluationService
    {
        IEnumerable<ISequence> Evaluate(ICommand command);
    }

    public class CommandEvaluationService : ICommandEvaluationService
    {
        private readonly IMap _map;

        public CommandEvaluationService(IMap map)
        {
            _map = map;
        }

        public IEnumerable<ISequence> Evaluate(ICommand command)
        {
            var affectedUnits = command.Evaluate();
            foreach (var unit in affectedUnits)
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