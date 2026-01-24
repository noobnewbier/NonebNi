using UnityEngine;

namespace NonebNi.Ui.Animation.Sequence
{
    public class KnockBackAnimSequence : IAnimSequence
    {
        public readonly Vector3 TargetPos;

        public KnockBackAnimSequence(Vector3 targetPos)
        {
            TargetPos = targetPos;
        }
    }
}