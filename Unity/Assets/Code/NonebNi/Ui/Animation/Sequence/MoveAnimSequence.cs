using System.Collections.Generic;
using UnityEngine;

namespace NonebNi.Ui.Animation.Sequence
{
    public class MoveAnimSequence : IAnimSequence
    {
        public readonly IEnumerable<Vector3> TargetPositions;

        public MoveAnimSequence(IEnumerable<Vector3> targetPositions)
        {
            TargetPositions = targetPositions;
        }
    }
}