using UnityEngine;

public class Player : MonoBehaviour 
{ 
    private Rigidbody rb;
    // movement
    public float moveSpeed = 5f;
    public KeyCode forwareds = KeyCode.W;
    public KeyCode left = KeyCode.A;
    public KeyCode back = KeyCode.S;
    public KeyCode right = KeyCode.D;
    // jumping
    public float jumpForce = 10f;
    public float fallMultiplier = 2.5f;
    public float ascendMultiplier = 2f;
    public KeyCode jumpKey = KeyCode.Space;

    // ground check
    public LayerMask groundLayer;
    private bool isGrounded;
    private float raycastDistance;

    
    void Start()
    {
        // Rigidbody setup
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        float playerHeight = GetComponent<CapsuleCollider>().height;
        raycastDistance = (playerHeight / 2) + 0.2f;
    }

    void Update()
    {
        // Ground check and jump input
        CheckGround();
        if (Input.GetKeyDown(jumpKey) && isGrounded) Jump();
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

        // smooth movement
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        Vector3 desiredVelocity = moveDirection * moveSpeed;
        Vector3 currentVelocity = rb.linearVelocity;
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
}
