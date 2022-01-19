using UnityEngine;
using UnityUtils.SerializableGuid;

namespace NonebNi.Ui.Entities
{
    public class Entity : MonoBehaviour
    {
        public SerializableGuid guid;

        public T GetAnimationControl<T>() where T : IAnimationControl => GetComponent<T>();
    }
}