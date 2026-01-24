using NonebNi.Ui.Animation;
using NonebNi.Ui.Animation.Sequence;
using UnityEngine;
using UnityUtils.Serialization;

namespace NonebNi.Ui.Entities
{
    [SelectionBase]
    public class Entity : MonoBehaviour
    {
        public SerializableGuid guid;

        public IPlayAnimation<T>? GetAnimationControl<T>() where T : IAnimSequence => GetComponentInChildren<IPlayAnimation<T>>();
    }
}