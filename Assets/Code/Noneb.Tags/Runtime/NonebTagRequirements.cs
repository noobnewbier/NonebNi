using System;
using UnityEngine;

namespace Noneb.Tags.Runtime
{
    [Serializable]
    public struct NonebTagRequirements
    {
        [SerializeField] private NonebTagContainer forbiddenTags;

        [SerializeField] private NonebTagContainer requiredTags;

        public NonebTagRequirements(NonebTagContainer forbiddenTags, NonebTagContainer requiredTags)
        {
            this.forbiddenTags = forbiddenTags;
            this.requiredTags = requiredTags;
        }

        public readonly bool Matches(in NonebTagContainer container) => !container.HasAny(forbiddenTags) && container.HasAll(requiredTags);
    }
}