using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly string? TargetLayerName;

        public AnimatorStateAttribute(string animatorName, string? targetLayerName = null)
        {
            AnimatorName = animatorName;
            TargetLayerName = targetLayerName;
        }
    }
}