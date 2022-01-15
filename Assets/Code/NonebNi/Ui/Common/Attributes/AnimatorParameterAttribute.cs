using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorParameterAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly AnimatorControllerParameterType ParameterType;

        public AnimatorParameterAttribute(string animatorName, AnimatorControllerParameterType aType)
        {
            AnimatorName = animatorName;
            ParameterType = aType;
        }
    }
}