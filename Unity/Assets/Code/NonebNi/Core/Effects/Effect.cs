using System;

namespace NonebNi.Core.Effects
{
    [Serializable]
    public abstract class Effect
    {
        //TODO: we need a way to find a way to figure out if the effect is valid,
        //and in some case bubble the error up and disable the action itself(whilst in some case it can proceeds)
        /// <summary>
        /// The thing you need to remember future me,
        /// is that if you are trying to design a super generic system that's going to fit every single use case including those
        /// that you don't know if it exists or not
        /// it's probably not going to end very well.
        /// The current way of doing it means each evaluator expects a certain format from the input(EffectContext), and it's okay?
        /// I mean I am totally up for a better approach but can't really come up with anything other than manually scripting
        /// everything and avoid script reuse.
        /// With the current way, if there's anything that we just cannot adopt with the existing set of evaluator, we can just
        /// create a new one, and it's fine
        /// </summary>
        public abstract class Evaluator<T> : IEffectEvaluator<T> where T : Effect
        {
            public EffectResult Evaluate(T effect, EffectContext context) => OnEvaluate(effect, context);

            //TODO: we can use a context/blackboard system, it won't be so bad would it? We are already halfway there anyway...?
            //TODO: document the fact that effect expect certain format in the param targets - this seems dangerous anyway, is there a way to get away from it?
            //Follow Up: perhaps a better way to do this is to have effect created with all its dependencies from the get go, instead of getting them passed in... also avoid the "where do i get services" issue?
            //No it doesn't because a) you now lost the way to define effect in pure data(unless you introduce EffectData class), b) you probs just kicked the prob upwards?
            //TODO: we can actually make all these typed parameter - at the risk of runtime exception(which we can catch)...
            protected abstract EffectResult OnEvaluate(T effect, EffectContext context);
        }
    }
}