using UnityEngine;

namespace Noneb.UI.Animation.Attributes
{
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly AnimatorControllerParameterType? ParameterType;
        public readonly bool UseRootObjectField;

        public AnimatorParameterAttribute(bool useRootObjectField = true) : this(
            string.Empty,
            null,
            useRootObjectField
        ) { }

        public AnimatorParameterAttribute(AnimatorControllerParameterType type, bool useRootObjectField = true) : this(
            string.Empty,
            type,
            useRootObjectField
        ) { }

        public AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType type, bool useRootObjectField = false)
        {
            ParameterType = type;
            UseRootObjectField = useRootObjectField;
            AnimatorName = animatorName;
        }

        private AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType? type, bool useRootObjectField = false)
        {
            ParameterType = type;
            UseRootObjectField = useRootObjectField;
            AnimatorName = animatorName;
        }
    }
}