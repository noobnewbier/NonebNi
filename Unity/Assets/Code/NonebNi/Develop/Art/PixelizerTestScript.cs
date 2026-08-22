using UnityEngine;

namespace NonebNi.Develop.Art
{
    public class PixelizerTestScript : MonoBehaviour
    {
        [SerializeField] private float movementRange;
        [SerializeField] private float moveSpeed;
        [SerializeField] private Transform movedObject = null!;

        private Vector3 _center;
        private float _angle;

        private void Awake()
        {
            _center = movedObject.transform.position;
        }

        private void Update()
        {
            _angle += moveSpeed / (movementRange * 2f * Mathf.PI) * Time.deltaTime;
            transform.position = _center + new Vector3(Mathf.Cos(_angle), 0, Mathf.Sin(_angle)) * movementRange;
        }
    }
} 