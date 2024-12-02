using System;
using NonebNi.Ui.Common.Attributes;
using UnityEngine;

namespace NonebNi.Ui.Animation
{
    /// <summary>
    /// goal - a generic record that can be used for any animator control that's related to animator parameter
    /// might need different type...?
    /// likely need a anim id dataref like support...?
    /// why i want this:
    /// 1. defining anim id is clumsy
    /// 2. defining transition is clumsy - but then some transition is complicated and trying to consolidate them could be
    /// futile.
    /// 3. it feels like a cool idea
    /// 4. but fuck if it's a bool we need to turn it off so it's never so simple
    /// 5. following this up we need multiple data class
    /// 6. and some of them is fire and forget(most likely trigger)
    /// 7. finish timing can differ per anim and might need cutoff halfway.
    /// 8. the more i type the more i think i need more requirement details before i do it.
    /// dude, don't aim for perfection, good enough is good enough, let's make something commit push and move on, we will come
    /// back to it later.
    /// this doesn't needs to be type safe, even if it looks type safe we will need a runtime type check anyway.
    /// Unless you intend to have code that strictly uses a specific type of animation data, but then it weakens the reason why
    /// you are making this in the first place
    /// (to make something generic so change in animator doesn't always necessitate change in code)
    /// </summary>
    [Serializable]
    public record AnimationData
    {
        // universally available
        [field: SerializeField, AnimatorParameter] public string Name { get; private set; } = null!;
        [field: SerializeField] private AnimatorControllerParameterType type;

        // These values are only used when they makes sense 
        [field: SerializeField, AnimatorState(nameof(FinishAnimLayerIndex))] public string FinishAnimState { get; private set; } = null!;
        [field: SerializeField, AnimatorLayer] public int FinishAnimLayerIndex { get; private set; }
        [field: SerializeField] public float TargetNumericValue { get; private set; }
        [field: SerializeField] public bool TargetBoolValue { get; private set; }


        public AnimatorControllerParameterType? GetParameterType()
        {
            if (string.IsNullOrEmpty(Name))
                //Invalid - nothing about this would work
                return null;

            return type;
        }
    }
}