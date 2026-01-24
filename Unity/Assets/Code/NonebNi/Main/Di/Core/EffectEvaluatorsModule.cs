using NonebNi.Core.Effects;
using StrongInject;
using StrongInject.Modules;

namespace NonebNi.Main.Di.Core
{
    [
        RegisterModule(typeof(CollectionsModule)),
        Register(typeof(DamageEffect.Evaluator), typeof(IEffectEvaluator), typeof(DamageEffect.Evaluator)),
        Register(typeof(KnockBackEffect.Evaluator), typeof(IEffectEvaluator), typeof(KnockBackEffect.Evaluator)),
        Register(typeof(MoveEffect.Evaluator), typeof(IEffectEvaluator), typeof(MoveEffect.Evaluator)),
        Register(typeof(MoveEntityEffect.Evaluator), typeof(IEffectEvaluator), typeof(MoveEntityEffect.Evaluator)),
        Register(typeof(MoveOverEffect.Evaluator), typeof(IEffectEvaluator), typeof(MoveOverEffect.Evaluator)),
        Register(typeof(PullEntityEffect.Evaluator), typeof(IEffectEvaluator), typeof(PullEntityEffect.Evaluator)),
        Register(typeof(SwapPositionEffect.Evaluator), typeof(IEffectEvaluator), typeof(SwapPositionEffect.Evaluator))
    ]
    public class EffectEvaluatorsModule { }
}