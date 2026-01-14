using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour 
{ 
    private Rigidbody rb;
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float ascendMultiplier = 2f;
    public LayerMask groundLayer;
    private float verticalRotation = 0f;
    private Transform cameraTransform;
    private bool isGrounded;
    private float raycastDistance;
    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        cameraTransform = Camera.main.transform;
        float playerHeight = GetComponent<CapsuleCollider>().height;
        raycastDistance = (playerHeight / 2) + 0.2f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    { 

        RotateCamera();
        CheckGround();
        if (Input.GetButtonDown("Jump") && isGrounded) Jump();
    }
    void FixedUpdate() 
    { 
        MovePlayer();
        ApplyJumpPhysics();
    }
    void MovePlayer()
    { 
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveForward = Input.GetAxisRaw("Vertical");
        Vector3 movement = (transform.right * moveHorizontal + transform.forward * moveForward).normalized;
        Vector3 desiredVelocity = movement * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }
    void RotateCamera() 
    { 
        float horizontalRotation = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(0, horizontalRotation, 0); verticalRotation -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0, 0); 
    } 
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }
    void ApplyJumpPhysics() 
    { 
        if (rb.linearVelocity.y < 0)
        { 
            rb.linearVelocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1)) * Time.fixedDeltaTime;
        } 
        else if (rb.linearVelocity.y > 0)
        { 
            rb.linearVelocity += Vector3.up * (Physics.gravity.y * (ascendMultiplier - 1)) * Time.fixedDeltaTime; 
        }
    }
    void CheckGround() 
    { Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, raycastDistance, groundLayer); 
    } 
}
