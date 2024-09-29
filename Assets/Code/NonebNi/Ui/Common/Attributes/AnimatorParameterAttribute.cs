using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly AnimatorControllerParameterType ParameterType;
        public readonly bool UseRootObjectField;

        public AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType aType, bool useRootObjectField = false)
        {
            AnimatorName = animatorName;
            ParameterType = aType;
            UseRootObjectField = useRootObjectField;
        }
    }
}