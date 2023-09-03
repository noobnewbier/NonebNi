using System;
using System.Collections.Generic;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Actions.Effects
{
    [Serializable]
    public abstract class Effect
    {
        public IEnumerable<ISequence> Evaluate(
            IMap map,
            EntityData actionCaster,
            //TODO: document the fact that effect expect certain format in the param targets - this seems dangerous anyway, is there a way to get away from it?
            IEnumerable<IActionTarget> targets) =>
            OnEvaluate(map, actionCaster, targets);

        protected abstract IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets);
    }
}