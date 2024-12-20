using NonebNi.Ui.Animation;
using UnityEngine;
using UnityUtils.Serialization;

namespace NonebNi.Ui.Entities
{
    [SelectionBase]
    public class Entity : MonoBehaviour
    {
        public SerializableGuid guid;

        public T GetAnimationControl<T>() where T : IPlayAnimation => GetComponentInChildren<T>();
    }
}