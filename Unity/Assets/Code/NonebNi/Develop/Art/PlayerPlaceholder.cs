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
    
    
    public float jumpForce = 5f;
    public LayerMask groundLayer;

    [Header("Ground Check")]
    [SerializeField] private float _groundCheckRadius = 0.3f;
    [SerializeField] private float _groundCheckDistance = 0.2f;

    private Rigidbody _rb;
    private bool _isGrounded;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        if (_rb == null)
        {
            Debug.LogError("[Player] Rigidbody not found!");
            return;
        }

        // Apply ideal physics settings for the player.
        _rb.freezeRotation = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
        _rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        Debug.Log("[Player] Placeholder initialized successfully.");
    }

    private void Update()
    {
        CheckGrounded();

        // Check jump input in Update for better responsiveness.
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CheckGrounded()
    {
        // Spherecast downwards to detect ground.
        _isGrounded = Physics.SphereCast(
            transform.position,
            _groundCheckRadius,
            Vector3.down,
            out RaycastHit hit,
            _groundCheckDistance,
            groundLayer
        );
    }

    private void Move()
    {
        float h = Input.GetAxis("Horizontal");
        h = Math.Sign(h);
        if (forceHoriz)
        {
            h = 1;
            if (forceReversed)
            {
                h = -1;
            }
        }
        float v = Input.GetAxis("Vertical");
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
        Vector3 camForward = camTrans.forward.ProjectOntoPlane(transform.up).normalized;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = camTrans.right.ProjectOntoPlane(transform.up).normalized;
        camRight.y = 0f;
        camRight.Normalize();


        // Calculate direction based on camera perspective.
        Vector3 moveDir = (camForward * v + camRight * h).normalized;

        // Use linearVelocity for Unity 6 compatibility.
        _rb.position = (_rb.position+ moveDir* moveSpeed  * Time.deltaTime);
    }

    private void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}