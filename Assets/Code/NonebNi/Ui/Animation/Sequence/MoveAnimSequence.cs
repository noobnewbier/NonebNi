using UnityEngine;

namespace NonebNi.Ui.Animation.Sequence
{
    public class MoveAnimSequence : IAnimSequence
    {
        public readonly Vector3 TargetPos;
        
        public MoveAnimSequence(Vector3 targetPos)
        {
            TargetPos = targetPos;
        }
    }
}