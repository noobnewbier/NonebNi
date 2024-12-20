using System;
using System.Collections.Generic;
using NonebNi.Core.Sequences;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public abstract class Effect
    {
        //TODO: we need a way to find a way to figure out if the effect is valid,
        //and in some case bubble the error up and disable the action itself(whilst in some case it can proceeds) 
        public abstract class Evaluator<T> : IEffectEvaluator<T> where T : Effect
        {
            public IEnumerable<ISequence> Evaluate(T effect, EffectContext context) => OnEvaluate(effect, context);

            //TODO: document the fact that effect expect certain format in the param targets - this seems dangerous anyway, is there a way to get away from it?
            //Follow Up: perhaps a better way to do this is to have effect created with all its dependencies from the get go, instead of getting them passed in... also avoid the "where do i get services" issue?
            //No it doesn't because a) you now lost the way to define effect in pure data(unless you introduce EffectData class), b) you probs just kicked the prob upwards?
            //TODO: we can actually make all these typed parameter - at the risk of runtime exception(which we can catch)...
            protected abstract IEnumerable<ISequence> OnEvaluate(T effect, EffectContext context);
        }
    }
}