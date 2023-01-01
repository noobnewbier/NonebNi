using UnityEngine;

namespace NonebNi.Ui.Animation.Sequence
{
    public class TeleportAnimSequence : IAnimSequence
    {
        public readonly Vector3 TargetTilePosition;

        public TeleportAnimSequence(Vector3 targetTilePosition)
        {
            TargetTilePosition = targetTilePosition;
        }
    }
}