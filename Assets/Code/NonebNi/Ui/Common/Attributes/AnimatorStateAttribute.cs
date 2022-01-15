using UnityEngine;

namespace NonebNi.Ui.Common.Attributes
{
    public class AnimatorStateAttribute : PropertyAttribute
    {
        public readonly string AnimatorName;
        public readonly int StateLayerIndex; //-1 if it's not specified

        public AnimatorStateAttribute(string animatorName, int layerIndex = -1)
        {
            AnimatorName = animatorName;
            StateLayerIndex = layerIndex;
        }
    }
}