using System;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPlaceholder : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    [SerializeField] private bool forceHoriz;
    [SerializeField] private bool forceVert;
    [SerializeField] private bool forceReversed;
    
    private bool _isGrounded;
    private void FixedUpdate()
    {
        Move();
    }


    private void Move()
    {
        var h = Input.GetAxis("Horizontal");
        h = Math.Sign(h);
        if (forceHoriz)
        {
            h = 1;
            if (forceReversed)
            {
                h = -1;
            }
        }
        var v = Input.GetAxis("Vertical");
        if (forceVert)
        {
            v = 1;
            if (forceReversed)
            {
                v = -1;
            }
        }
        
        // Project camera vectors onto the XZ plane.
        var camTrans = Camera.main.transform;
        var camForward = camTrans.forward.ProjectOntoPlane(transform.up).normalized;
        camForward.y = 0f;
        camForward.Normalize();

        var camRight = camTrans.right.ProjectOntoPlane(transform.up).normalized;
        camRight.y = 0f;
        camRight.Normalize();


        // Calculate direction based on camera perspective.
        var moveDir = (camForward * v + camRight * h).normalized;

        // Use linearVelocity for Unity 6 compatibility.
        transform.position = (transform.position+ moveDir* moveSpeed  * Time.fixedDeltaTime);
    }
}