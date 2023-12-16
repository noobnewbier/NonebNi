using System;
using NonebNi.Core.Effects;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public int Range { get; private set; }
        [field: SerializeField] public TargetRestriction TargetRestriction { get; private set; }
        [field: SerializeField] public TargetArea TargetArea { get; private set; }
        [field: SerializeField] public string RangeModifierId { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }

        public NonebAction(
            string id,
            int range,
            TargetRestriction targetRestriction,
            TargetArea targetArea,
            string rangeModifierId,
            int fatigueCost,
            Effect[] effects)
        {
            Id = id;
            Range = range;
            TargetRestriction = targetRestriction;
            TargetArea = targetArea;
            RangeModifierId = rangeModifierId;
            FatigueCost = fatigueCost;
            Effects = effects;
        }

        [field: SerializeField] public Effect[] Effects { get; private set; }
        public override string ToString() => Id;
    }
}