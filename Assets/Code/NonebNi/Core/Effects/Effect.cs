﻿using System;
using System.Collections.Generic;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Maps;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Effects
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

        //TODO: we can actually make all these typed parameter - at the risk of runtime exception(which we can catch)...
        protected abstract IEnumerable<ISequence> OnEvaluate(
            IMap map,
            EntityData actionCaster,
            IEnumerable<IActionTarget> targets);
    }
}