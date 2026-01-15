using UnityEngine;

public enum PlayerState 
{ 
    Idle, 
    Run, 
    Jump, 
    Crouch, 
    Slide 
}
public class Player : MonoBehaviour 
{ 
    // Components
    private Rigidbody rb; 
    private CapsuleCollider capsule; 

    // State Machine
    public PlayerState currentState = PlayerState.Idle; 

    // Movement
    public float moveSpeed = 5f; 
    private Vector3 moveDirection; 
    public KeyCode forwardKey = KeyCode.W; 
    public KeyCode leftKey = KeyCode.A; 
    public KeyCode backKey = KeyCode.S; 
    public KeyCode rightKey = KeyCode.D;

    // Jumping
    public float jumpForce = 10f; 
    public float fallMultiplier = 2.5f; 
    public float ascendMultiplier = 2f; 
    public KeyCode jumpKey = KeyCode.Space; 

    // Crouch & Slide
    public float crouchHeight = 1f; 
    public float crouchSpeed = 5f; 
    public KeyCode crouchKey = KeyCode.LeftControl; 
    public float slideSpeed = 15f; 
    public float slideDuration = 1f; 
    private float slideTimer; 
    private Vector3 slideDirection;
    public float slideMinSpeed = 4f;

    //Ground & Ceiling
    public LayerMask groundLayer; 
    public LayerMask ceilingMask; 
    public float ceilingCheckDistance = 0.1f; 
    private bool isGrounded; 
    private float playerHeight;
    private float targetHeight; 

    // ????????????????????????????????????????????? 
    // UNITY METHODS 
    // ?????????????????????????????????????????????
    void Start() 
    {
        rb = GetComponent<Rigidbody>(); 
        rb.freezeRotation = true; 
        capsule = GetComponent<CapsuleCollider>(); 
        playerHeight = capsule.height; 
        targetHeight = playerHeight; 
    } 
    
    void Update() 
    { 
        CheckGround();
        ReadMovementInput(); 
        HandleStateTransitions(); 
        SmoothCrouchHeight(); 
    } 

    void FixedUpdate() 
    { 
        ApplyStateMovement(); 
        ApplyJumpPhysics();
    } 

    // ????????????????????????????????????????????? 
    // INPUT 
    // ?????????????????????????????????????????????
    void ReadMovementInput() 
    { 
        moveDirection = Vector3.zero; 
        if (Input.GetKey(forwardKey)) 
            moveDirection += Vector3.forward; 
        if (Input.GetKey(backKey)) 
            moveDirection += Vector3.back; 
        if (Input.GetKey(leftKey)) 
            moveDirection += Vector3.left; 
        if (Input.GetKey(rightKey)) 
            moveDirection += Vector3.right; 
        if (moveDirection.magnitude > 1f) 
            moveDirection.Normalize();
    } 

    // ????????????????????????????????????????????? 
    // STATE TRANSITIONS 
    // ?????????????????????????????????????????????
    void HandleStateTransitions() 
    { 
        switch (currentState) 
        { 
            case PlayerState.Idle: 
                if (moveDirection.magnitude > 0.1f) 
                    currentState = PlayerState.Run; 
                if (Input.GetKeyDown(jumpKey) && isGrounded) 
                    currentState = PlayerState.Jump;
                if (isGrounded) 
                {
                    if (Input.GetKeyDown(crouchKey))
                        currentState = PlayerState.Crouch;
                }
                
            break; 

            case PlayerState.Run: 
                if (moveDirection.magnitude <= 0.1f) 
                    currentState = PlayerState.Idle; 
                if (Input.GetKeyDown(jumpKey) && isGrounded) 
                    currentState = PlayerState.Jump;

                if (isGrounded)
                {
                    if (Input.GetKeyDown(crouchKey))
                    {
                        if (rb.linearVelocity.magnitude > slideMinSpeed)
                            StartSlideState();
                        else currentState = PlayerState.Crouch;
                    }
                }
            break; 

            case PlayerState.Jump: 
                if (isGrounded) currentState = moveDirection.magnitude > 0.1f ? PlayerState.Run : PlayerState.Idle; 
            break; 

            case PlayerState.Crouch:
                // If crouch key released ? try to stand
                if (Input.GetKeyUp(crouchKey)) 
                { 
                    TryStand(); 
                    break; 
                } 
                // If crouch key pressed again while moving fast ? slide
                if (Input.GetKeyDown(crouchKey) && rb.linearVelocity.magnitude > slideMinSpeed) 
                { StartSlideState();
                    break; 
                } 
                // Stay crouched only while key is held
                Crouch(); 
            break;

            case PlayerState.Slide: 
                slideTimer -= Time.deltaTime; 
                if (slideTimer <= 0)
                {
                    EndSlideState(); 
                } 

                if(Input.GetKeyUp(crouchKey))
                {
                    EndSlideState();
                    TryStand();
                }
            break; 
        } 
    } 

    // ????????????????????????????????????????????? 
    // STATE MOVEMENT 
    // ?????????????????????????????????????????????
    void ApplyStateMovement() 
    { 
        switch (currentState) 
        { 
            case PlayerState.Idle: 
            case PlayerState.Run: 
                MoveNormally();
            break;

            case PlayerState.Jump: 
                MoveNormally(); 
            break; 

            case PlayerState.Crouch:
                ApplyCrouchMovement();
                break; 

            case PlayerState.Slide: 
                rb.AddForce(slideDirection * slideSpeed * Time.fixedDeltaTime, ForceMode.Acceleration);
            break; 
        } 
    } 

    // ????????????????????????????????????????????? 
    // MOVEMENT HELPERS 
    // ?????????????????????????????????????????????
    void MoveNormally() 
    { 
        Vector3 desiredVelocity = moveDirection * moveSpeed; 
        Vector3 currentVelocity = rb.linearVelocity; 
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z); rb.AddForce(velocityChange, ForceMode.VelocityChange); 
        if (Input.GetKey(jumpKey) && isGrounded) Jump(); 
    } 

    void Jump() 
    { 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z); 
    } 

    void ApplyJumpPhysics() 
    { 
        if (rb.linearVelocity.y < 0) 
            rb.linearVelocity += Vector3.up * (Physics.gravity.y * (fallMultiplier - 1)) * Time.fixedDeltaTime; 
        else if (rb.linearVelocity.y > 0) 
            rb.linearVelocity += Vector3.up * (Physics.gravity.y * (ascendMultiplier - 1)) * Time.fixedDeltaTime; 
    } 

    // ????????????????????????????????????????????? 
    // CROUCH & SLIDE 
    // ?????????????????????????????????????????????
    void StartSlideState() 
    { 
        currentState = PlayerState.Slide; 
        slideDirection = moveDirection.magnitude > 0.1f ? moveDirection.normalized : transform.forward; 
        slideTimer = slideDuration; 
        targetHeight = crouchHeight; 
        rb.AddForce(slideDirection * slideSpeed, ForceMode.VelocityChange); 
    } 

    void Crouch()
    {
        targetHeight = crouchHeight;
    }

    void EndSlideState() 
    {
        // If crouch key is still held ? crouch
        if (Input.GetKey(crouchKey)) 
        { 
            currentState = PlayerState.Crouch;
            return; 
        } 
        // Otherwise stand up
        TryStand();
    }

    void TryStand() 
    { 
        bool blocked = Physics.Raycast(transform.position, Vector3.up, (playerHeight - crouchHeight) / 2 + ceilingCheckDistance, ceilingMask); 
        if (!blocked) 
        { 
            targetHeight = playerHeight;
            if (moveDirection.magnitude > 0.1f) 
                currentState = PlayerState.Run; 
            else 
                currentState = PlayerState.Idle;
        }
        Debug.Log("TryStand called. Blocked = " + blocked);
        Debug.Log("Standing. targetHeight = " + targetHeight);
        Debug.DrawRay(transform.position + Vector3.up * (crouchHeight / 2f), Vector3.up * ((playerHeight - crouchHeight) + ceilingCheckDistance), blocked ? Color.red : Color.green);
    } 

    void SmoothCrouchHeight() 
    { 
        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * crouchSpeed); 
    } 

    // ????????????????????????????????????????????? 
    // GROUND CHECK 
    // ?????????????????????????????????????????????
    void CheckGround() 
    { 
        Vector3 origin = transform.position + Vector3.up * 0.1f; 
        isGrounded = Physics.Raycast(origin, Vector3.down, playerHeight / 2 + 0.2f, groundLayer);
    }

    void ApplyCrouchMovement()
    {
        // Keep existing momentum
        Vector3 currentVelocity = rb.linearVelocity;
        // Optional: allow slow steering while crouched
        Vector3 steer = moveDirection * (moveSpeed * 0.15f);
        // 30% steering
        Vector3 newVelocity = new Vector3(
            currentVelocity.x + (steer.x * Time.fixedDeltaTime),
            currentVelocity.y,
            currentVelocity.z + (steer.z * Time.fixedDeltaTime)
        );

        rb.linearVelocity = newVelocity;
        // Still allow jumping if you want
        if (Input.GetKey(jumpKey) && isGrounded)
            Jump();
    }
}