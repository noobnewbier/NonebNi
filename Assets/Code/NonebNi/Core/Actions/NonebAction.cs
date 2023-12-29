using System;
using NonebNi.Core.Effects;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public TargetRestriction[] TargetRestrictions { get; private set; }
        [field: SerializeField] public TargetArea TargetArea { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects)
        {
            Id = id;
            Ranges = new[] { range };
            TargetRestrictions = targetRestrictions;
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Effects = effects;
        }

        public NonebAction(
            string id,
            Range[] ranges,
            TargetRestriction[] targetRestrictions,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects)
        {
            Id = id;
            Ranges = ranges;
            TargetRestrictions = targetRestrictions;
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Effects = effects;
        }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction targetRestriction,
            TargetArea targetArea,
            int fatigueCost,
            params Effect[] effects)
        {
            Id = id;
            Ranges = new[] { range };
            TargetRestrictions = new[] { targetRestriction };
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Effects = effects;
        }

        [field: SerializeReference] public Range[] Ranges { get; private set; }

        public override string ToString() => Id;
    }
}