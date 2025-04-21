using System.Linq;
using NonebNi.Core.Actions;
using Unity.Logging;

namespace NonebNi.Core.Effects
{
    public record EffectTargetGroup(IActionTarget[] Targets)
    {
        public IActionTarget? AsSingleTarget
        {
            get
            {
                switch (Targets.Length)
                {
                    case > 1:
                        Log.Warning("Multiple targets found when the code really just wanted one target, something went wrong");
                        break;
                    case 0:
                        Log.Warning("My gut feeling is that you should never be here, I can't really fathom why would this be a valid case right now, but maybe I was wrong, future me glhf");
                        break;
                }

                return Targets.FirstOrDefault();
            }
        }
    }
}