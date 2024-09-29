using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly string? TargetLayerName;
        public readonly bool UseRootObjectField;

        public AnimatorStateAttribute(string animatorName, string? targetLayerName = null, bool useRootObjectField = false)
        {
            AnimatorName = animatorName;
            UseRootObjectField = useRootObjectField;
            TargetLayerName = targetLayerName;
        }
    }
}