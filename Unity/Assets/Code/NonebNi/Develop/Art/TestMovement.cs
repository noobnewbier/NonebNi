using System;
using UnityEngine;

namespace NonebNi.Develop.Art
{
    public class TestMovement : MonoBehaviour
    {
        public float moveSpeed = 5f;
        [SerializeField] private bool forceMovement;
        [SerializeField] private bool forceReversed;

        private void FixedUpdate()
        {
            Move();
        }

        private void Move()
        {
            float h = Input.GetAxis("Horizontal");
            h = Math.Sign(h);
            if (forceMovement)
            {
                h = 1;
                if (forceReversed)
                {
                    h = -1;
                }
            }
            float v = Input.GetAxis("Vertical");

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            Vector3 moveDir = (forward * v + right * h).normalized;

            // Use linearVelocity for Unity 6 compatibility.
            transform.position = (transform.position+ moveDir* moveSpeed  * Time.deltaTime);
        }
    }
}