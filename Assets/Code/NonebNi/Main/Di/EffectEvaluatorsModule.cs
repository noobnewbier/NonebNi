using NonebNi.Core.Effects;
using StrongInject;
using StrongInject.Modules;

namespace NonebNi.Main.Di
{
    [
        RegisterModule(typeof(CollectionsModule)),
        Register(typeof(DamageEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(KnockBackEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(MoveEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(MoveEntityEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(MoveOverEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(PullEntityEffect.Evaluator), typeof(IEffectEvaluator)),
        Register(typeof(SwapPositionEffect.Evaluator), typeof(IEffectEvaluator))
    ]
    public class EffectEvaluatorsModule { }
}