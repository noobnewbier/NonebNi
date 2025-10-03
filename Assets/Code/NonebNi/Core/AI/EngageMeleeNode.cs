using System;
using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Units;
using Action = Unity.Behavior.Action;

namespace NonebNi.Core.AI
{
    public class EngageMeleeNode : Action, IUtilityNode
    {
        //todo: act -> if enemy in range -> engage, else find heat and approach heat.
        public IAsyncEnumerable<(ActionCommand command, Utility utility)> FindActionAndUtility(UnitData controlledUnit) => throw new NotImplementedException();
    }
}