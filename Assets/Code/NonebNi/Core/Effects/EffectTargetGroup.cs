using System.Linq;
using NonebNi.Core.Actions;

namespace NonebNi.Core.Effects
{
    public record EffectTargetGroup(IActionTarget[] Targets)
    {
        public IActionTarget? AsSingleTarget => Targets.FirstOrDefault();
    }
}