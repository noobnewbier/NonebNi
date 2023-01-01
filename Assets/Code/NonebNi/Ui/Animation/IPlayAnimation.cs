using NonebNi.Ui.Animation.Sequence;
using UnityEngine;

namespace NonebNi.Ui.Animation
{
    public interface IPlayAnimation { }

    public interface IPlayAnimation<in T> : IPlayAnimation where T : IAnimSequence
    {
        Coroutine Play(T sequence);
    }
}