using System;
using NonebNi.Core.Commands;
using NonebNi.Core.Maps;
using NonebNi.Core.Stats;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class ConstantStatRequirement : StatRequirement
    {
        [SerializeField] private StatCost cost;

        public ConstantStatRequirement(StatCost cost)
        {
            this.cost = cost;
        }

        public override StatCost CalculateCost(ActionCommand _, IReadOnlyMap __) => cost;
    }
}