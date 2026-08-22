using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NonebNi.Develop.Art
{
    public class TestCameraMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
        
        
        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard.wKey.isPressed)
            {
                transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
            }

            if (keyboard.sKey.isPressed)
            {
                transform.position +=  Vector3.back * moveSpeed * Time.deltaTime;
            }

            if (keyboard.aKey.isPressed)
            {
                transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            }

            if ( keyboard.dKey.isPressed)
            {
                transform.position += Vector3.right * moveSpeed * Time.deltaTime;
            }
        }
    }
}