using System;
using NonebNi.Core.Effects;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public class NonebAction
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public TargetRestriction TargetRestriction { get; private set; }
        [field: SerializeField] public TargetArea TargetArea { get; private set; }
        [field: SerializeField] public int FatigueCost { get; private set; }
        [field: SerializeReference] public Effect[] Effects { get; private set; }
        [field: SerializeReference] public Range Range { get; private set; }

        public NonebAction(
            string id,
            Range range,
            TargetRestriction targetRestriction,
            TargetArea targetArea,
            int fatigueCost,
            Effect[] effects)
        {
            Id = id;
            Range = range;
            TargetRestriction = targetRestriction;
            TargetArea = targetArea;
            FatigueCost = fatigueCost;
            Effects = effects;
        }

        public override string ToString() => Id;
    }
}