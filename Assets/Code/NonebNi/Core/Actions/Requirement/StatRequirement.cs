using System;
using NonebNi.Core.Commands;
using NonebNi.Core.Maps;
using NonebNi.Core.Stats;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public abstract class StatRequirement
    {
        public abstract StatCost CalculateCost(ActionCommand command, IReadOnlyMap map);
        
        public static implicit operator StatRequirement(StatCost cost) => new ConstantStatRequirement(cost);
    }
}