using System.Linq;
using Noneb.Logs.Runtime;
using NonebNi.Core.Actions;

namespace NonebNi.Core.Effects
{
    public record EffectTargetGroup(params IActionTarget[] Targets)
    {
        public IActionTarget? AsSingleTarget
        {
            get
            {
                switch (Targets.Length)
                {
                    case > 1:
                        Log.Warn("Effects", "Multiple targets found when the code really just wanted one target, something went wrong");
                        break;
                    case 0:
                        Log.Warn("Effects", "My gut feeling is that you should never be here, I can't really fathom why would this be a valid case right now, but maybe I was wrong, future me glhf");
                        break;
                }

                return Targets.FirstOrDefault();
            }
        }
    }
}