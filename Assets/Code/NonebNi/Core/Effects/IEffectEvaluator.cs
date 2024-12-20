using System.Collections.Generic;
using System.Linq;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Effects
{
    public interface IEffectEvaluator
    {
        public (bool isSuccess, IEnumerable<ISequence> sequences) Evaluate(Effect effect, EffectContext context);
    }

    public interface IEffectEvaluator<in T> : IEffectEvaluator where T : Effect
    {
        (bool isSuccess, IEnumerable<ISequence> sequences) IEffectEvaluator.Evaluate(Effect effect, EffectContext context)
        {
            if (effect is not T typedEffect) return (false, Enumerable.Empty<ISequence>());

            return (true, Evaluate(typedEffect, context));
        }

        public IEnumerable<ISequence> Evaluate(T effect, EffectContext context);
    }
}