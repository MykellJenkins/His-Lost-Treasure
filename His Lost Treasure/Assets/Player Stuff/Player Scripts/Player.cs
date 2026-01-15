using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour 
{
    private CapsuleCollider capsule;
    private Rigidbody rb;
    // movement
    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    public KeyCode forwareds = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    // jumping
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float ascendMultiplier = 2f;
    public KeyCode jumpKey = KeyCode.Space;

    public float crouchHeight = 1.0f;              
    public float crouchSpeed = 5f;          
    public KeyCode crouchKey = KeyCode.LeftControl;

    public float slideSpeed = 15f;
    public float slideDuration = 1f;
    private bool isSliding = false;
    private float slideTimer = 0f;


    public float ceilingCheckDistance = 0.1f; 
    public LayerMask ceilingMask;             

    
    private float targetHeight;
    private bool isCrouching;

    // ground check
    public LayerMask groundLayer;
    private bool isGrounded;
    private float raycastDistance;

    private float playerHeight;

    void Start()
    {
        // Rigidbody setup
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerHeight = GetComponent<CapsuleCollider>().height;
        raycastDistance = (playerHeight / 2) + 0.2f;

        // Capsule collider setup
        capsule = GetComponent<CapsuleCollider>();
        targetHeight = playerHeight;
        capsule.height = playerHeight;

    }

    void Update()
    {
        // Ground check and jump input
        CheckGround();
        if (Input.GetKeyDown(jumpKey) && isGrounded) Jump();

        if (Input.GetKeyDown(crouchKey))
        {
            StartCrouch();
        }
        else if (Input.GetKeyUp(crouchKey))
        {
            TryStand();
        }

        if (isSliding == true)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0)
            {
                EndSlide();
            }
        }

        // Smoothly adjust height
        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * crouchSpeed);
    }

    void FixedUpdate() 
    {
        //  Movement and jump physics
        MovePlayer();
        ApplyJumpPhysics();

    }

    // Movement
    void MovePlayer()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(forwareds))
        {
            moveDirection += Vector3.forward; // Move forward (Z+)
        }
        if (Input.GetKey(back))
        {
            moveDirection += Vector3.back; // Move backward (Z-)
        }
        if (Input.GetKey(left))
        {
            moveDirection += Vector3.left; // Move left (X-)
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveDirection += Vector3.right; // Move right (X+)
        }

        // Normalize to prevent faster diagonal movement
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        if (Input.GetKeyDown(crouchKey) && moveDirection.magnitude > 0 && isSliding == false)
        {
            StartSlide();
        }

        // smooth movement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        Vector3 desiredVelocity = moveDirection * moveSpeed;
        Vector3 currentVelocity;
        if (isSliding == false)
        {
            currentVelocity = rb.linearVelocity;
        }
        else 
        {
            
            currentVelocity = rb.linearVelocity * slideSpeed / 2;
        }

            Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    // Jumping
    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
    }

    // Better jump physics
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

    // Ground check using raycast
    void CheckGround() 
    { 
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        isGrounded = Physics.Raycast(origin, Vector3.down, raycastDistance, groundLayer); 
    }

    void StartCrouch()
    {
        if (isGrounded)
        {
            isCrouching = true;
            targetHeight = crouchHeight;
        }
        
    }

    void TryStand()
    {
        // Check if there is space above to stand
        if (!Physics.Raycast(transform.position, Vector3.up, (playerHeight - crouchHeight) / 2 + ceilingCheckDistance, ceilingMask))
        {
            isCrouching = false;
            targetHeight = playerHeight;
        }
        else
        {
            // Still under a ceiling, stay crouched
            isCrouching = true;
            targetHeight = crouchHeight;
        }
    }

    public bool IsCrouching()
    {
        return isCrouching;
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

    }

    void EndSlide()
    {
        isSliding = false;

    }
}
