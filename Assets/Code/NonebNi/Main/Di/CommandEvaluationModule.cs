using NonebNi.Core.Actions;
using NonebNi.Core.Decisions;
using NonebNi.Core.FlowControl;
using StrongInject;

namespace NonebNi.Main.Di
{
    [RegisterModule(typeof(EffectEvaluatorsModule))]
    [Register(typeof(ActionCommandEvaluator), typeof(IActionCommandEvaluator))]
    [Register(typeof(TargetFinder), typeof(ITargetFinder))]
    [Register(typeof(ActionOptionFinder), typeof(IActionOptionFinder))]
    public class CommandEvaluationModule { }
}