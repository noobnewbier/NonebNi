using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public readonly string AnimatorName = string.Empty;
        public readonly AnimatorControllerParameterType ParameterType;
        public readonly bool UseRootObjectField;

        public AnimatorParameterAttribute(AnimatorControllerParameterType type, bool useRootObjectField = true)
        {
            ParameterType = type;
            UseRootObjectField = useRootObjectField;
        }

        public AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType type, bool useRootObjectField = false) : this(
            type,
            useRootObjectField
        )
        {
            AnimatorName = animatorName;
        }
    }
}