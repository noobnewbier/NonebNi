using System;
using System.Collections.Generic;
using NonebNi.Core.Commands;
using NonebNi.Core.Units;
using Action = Unity.Behavior.Action;

namespace NonebNi.Core.AI
{
    // todo: engage, attack melee, and flee is the three basics we need to "have an AI" comon finish this and you have a prototype
    public class FleeNode : Action, IUtilityNode
    {
        public IAsyncEnumerable<(ActionCommand command, Utility utility)> FindActionAndUtility(UnitData controlledUnit) => throw new NotImplementedException();
    }
}