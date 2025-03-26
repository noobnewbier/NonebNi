using UnityEngine;

namespace Noneb.UI.Animation.Attributes
{
    public class AnimatorLayerAttribute : PropertyAttribute
    {
        public readonly string AnimatorName = string.Empty;
        public readonly bool UseRootObjectField;

        public AnimatorLayerAttribute(bool useRootObjectField = true)
        {
            UseRootObjectField = useRootObjectField;
        }

        public AnimatorLayerAttribute(string animatorName, bool useRootObjectField = false) : this(useRootObjectField)
        {
            AnimatorName = animatorName;
        }
    }
}