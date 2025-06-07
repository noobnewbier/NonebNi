using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Actions;
using NonebNi.Core.Entities;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Effects
{
    public interface IEffectEvaluator
    {
        public (bool isSuccess, EffectResult result) Evaluate(Effect effect, EffectContext context);
    }

    public interface IEffectEvaluator<in T> : IEffectEvaluator where T : Effect
    {
        (bool isSuccess, EffectResult result) IEffectEvaluator.Evaluate(Effect effect, EffectContext context)
        {
            if (effect is not T typedEffect) return (false, new EffectResult(Enumerable.Empty<ISequence>(), new HashSet<IActionTarget>(), new HashSet<EntityData>()));

            return (true, Evaluate(typedEffect, context));
        }

        public EffectResult Evaluate(T effect, EffectContext context);
    }
}