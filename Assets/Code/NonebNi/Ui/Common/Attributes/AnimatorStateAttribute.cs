using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public readonly string AnimatorName = string.Empty;
        public readonly string? TargetLayerName;
        public readonly bool UseRootObjectField;

        public AnimatorStateAttribute(string? targetLayerName = null, bool useRootObjectField = true)
        {
            UseRootObjectField = useRootObjectField;
            TargetLayerName = targetLayerName;
        }

        public AnimatorStateAttribute(string animatorName, string? targetLayerName = null, bool useRootObjectField = false) : this(
            targetLayerName,
            useRootObjectField
        )
        {
            AnimatorName = animatorName;
        }
    }
}