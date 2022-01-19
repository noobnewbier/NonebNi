using UnityEngine;

namespace NonebNi.Ui.Entities
{
    public interface IAnimationControl
    {
        Coroutine Play(Context context);
    }
}