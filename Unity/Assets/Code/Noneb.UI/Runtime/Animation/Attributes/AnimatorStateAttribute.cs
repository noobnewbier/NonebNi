using UnityEngine;

namespace Noneb.UI.Animation.Attributes
{
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly string? TargetLayerName;
        public readonly bool UseRootObjectField;

        public AnimatorStateAttribute(string? targetLayerName = null) : this(string.Empty, targetLayerName, true) { }

        public AnimatorStateAttribute(string animatorName, string? targetLayerName = null, bool useRootObjectField = false)
        {
            TargetLayerName = targetLayerName;
            UseRootObjectField = useRootObjectField;
            AnimatorName = animatorName;
        }
    }
}