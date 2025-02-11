using System;
using UnityEngine;

namespace NonebNi.Core.Actions
{
    [Serializable]
    public record TargetRequest
    {
        [field: SerializeField] public TargetRestriction TargetRestrictionFlags { get; private set; } = TargetRestriction.None;
        [field: SerializeField] public TargetArea TargetArea { get; private set; } = TargetArea.Single;
        [field: SerializeReference] public Range Range { get; private set; } = new ConstantRange(1);

        public TargetRequest(TargetRestriction targetRestrictionFlags, TargetArea targetArea, Range range)
        {
            TargetRestrictionFlags = targetRestrictionFlags;
            TargetArea = targetArea;
            Range = range;
        }
    }
}