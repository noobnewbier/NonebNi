using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorLayerAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;

        public AnimatorLayerAttribute(string animatorName)
        {
            AnimatorName = animatorName;
        }
    }
}