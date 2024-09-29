using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorLayerAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly bool UseRootObjectField;

        public AnimatorLayerAttribute(string animatorName, bool useRootObjectField = false)
        {
            AnimatorName = animatorName;
            UseRootObjectField = useRootObjectField;
        }
    }
}