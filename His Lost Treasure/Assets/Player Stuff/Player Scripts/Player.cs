using UnityEngine;

public enum PlayerState 
{ 
    Idle, 
    Run, 
    Jump, 
    Crouch, 
    Slide,
    Sprint,
    Damage
}
public class Player : MonoBehaviour, IDamage
{ 
    // Components
    private Rigidbody rb; 
    private CapsuleCollider capsule;
    public Transform cam;

    // State Machine
    public PlayerState currentState = PlayerState.Idle; 

    // Lives
    public int maxLives = 3;
    public bool isHurt;

    // dmage effect
    public float damageStunDuration = 0.5f;
    private float damageTimer;
    private Vector3 damageSourcePosition;
    private PlayerDamageEffects damageEffects;

    // Movement
    public float moveSpeed = 5f; 
    private Vector3 moveDirection; 
    public KeyCode forwardKey = KeyCode.W; 
    public KeyCode leftKey = KeyCode.A; 
    public KeyCode backKey = KeyCode.S; 
    public KeyCode rightKey = KeyCode.D;

    // Sprinting
    public float SprintSpeed = 6f;
    public KeyCode RunKey = KeyCode.LeftShift;

    // Jumping
    public float jumpForce = 10f;
    public int jumpLeft = 2;
    public int jumps = 2;
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
        if (cam == null) cam = Camera.main.transform;
        damageEffects = GetComponent<PlayerDamageEffects>();
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
        float horizontal = 0;
        float vertical = 0;

        if (Input.GetKey(forwardKey)) vertical += 1;
        if (Input.GetKey(backKey)) vertical -= 1;
        if (Input.GetKey(leftKey)) horizontal -= 1;
        if (Input.GetKey(rightKey)) horizontal += 1;

        // 1. Get Camera directions
        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        // 2. Flatten them (set Y to 0) so player doesn't move into the floor
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Calculate direction based on flattened camera vectors
        moveDirection = (camForward * vertical) + (camRight * horizontal);

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        // 4. (Optional) Rotate player to face move direction
        if (moveDirection != Vector3.zero)
        {
            // Smoothly rotate the player model to face where they are moving
            transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * 10f);
        }
    } 

    // ????????????????????????????????????????????? 
    // STATE TRANSITIONS 
    // ?????????????????????????????????????????????
    void HandleStateTransitions() 
    {
        if (Input.GetKeyDown(jumpKey) && jumpLeft > 0)
        {
            currentState = PlayerState.Jump;
            Jump(); // Call the physics logic immediately
            return; // Exit early so we don't overwrite the state below
        }

        switch (currentState) 
        {

            case PlayerState.Idle:
                if (moveDirection.magnitude > 0.1f)
                {
                    // Check if we should start sprinting immediately from idle
                    currentState = PlayerState.Run;
                }
                //if (Input.GetKeyDown(jumpKey) && isGrounded && jumpLeft > 0) currentState = PlayerState.Jump;
                if (isGrounded && Input.GetKeyDown(crouchKey)) currentState = PlayerState.Crouch;
                if (isHurt == true) currentState = PlayerState.Damage;
            break;

            case PlayerState.Run:
                if (moveDirection.magnitude <= 0.1f) currentState = PlayerState.Idle;

                // Transition TO Sprint
                if (Input.GetKey(RunKey) && isGrounded) currentState = PlayerState.Sprint;
                if (isHurt == true) currentState = PlayerState.Damage;

                //if (Input.GetKeyDown(jumpKey) && isGrounded && jumpLeft > 0) currentState = PlayerState.Jump;
                if (isGrounded && Input.GetKeyDown(crouchKey))
                {
                    if (rb.linearVelocity.magnitude > slideMinSpeed) StartSlideState();
                    else currentState = PlayerState.Crouch;
                }
                break;

            case PlayerState.Sprint:
                // Transition OUT of Sprint
                if (moveDirection.magnitude <= 0.1f) currentState = PlayerState.Idle;
                if (!Input.GetKey(RunKey)) currentState = PlayerState.Run;
                if (isHurt == true) currentState = PlayerState.Damage;

                //if (Input.GetKeyDown(jumpKey) && isGrounded && jumpLeft > 0) currentState = PlayerState.Jump;
                if (isGrounded && Input.GetKeyDown(crouchKey)) StartSlideState(); // Sprinting usually slides
                break;

            case PlayerState.Jump: 
                if (isGrounded) 
                    currentState = moveDirection.magnitude > 0.1f ? PlayerState.Run : PlayerState.Idle; 
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
                { 
                    StartSlideState();
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

                if (isHurt == true)
                {
                    EndSlideState();
                    TryStand();
                    currentState = PlayerState.Damage;
                }
               
            break;

                case PlayerState.Damage:
                damageTimer -= Time.deltaTime;
                if (damageTimer <= 0 && isGrounded)
                {
                    isHurt = false;
                    currentState = PlayerState.Idle;
                }
                break;


        }
    } 

    // ????????????????????????????????????????????? 
    // STATE MOVEMENT 
    // ?????????????????????????????????????????????
    void ApplyStateMovement() 
    {
        if (currentState == PlayerState.Damage) return;
        switch (currentState) 
        {
            case PlayerState.Idle:
            case PlayerState.Run:
                Move(moveSpeed); // Pass the speed as a parameter
                break;

            case PlayerState.Sprint:
                Move(SprintSpeed);
                break;

            case PlayerState.Jump: 
                MoveNormally(); 
            break; 

            case PlayerState.Crouch:
                ApplyCrouchMovement();
                break; 

            case PlayerState.Slide:
                float forceMultiplier = slideTimer / slideDuration;
                rb.AddForce(slideDirection * slideSpeed * forceMultiplier, ForceMode.Acceleration);

                // Friction: Help the player slow down
                rb.linearDamping = 1f; // Use rb.drag in older Unity versions
                break; 
        } 
    }

    // ????????????????????????????????????????????? 
    // MOVEMENT HELPERS 
    // ?????????????????????????????????????????????
    void Move(float speed)
    {
        Vector3 desiredVelocity = moveDirection * speed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
        if (Input.GetKeyDown(jumpKey) && jumpLeft > 0) Jump();
    }
    void MoveNormally() 
    { 
        Vector3 desiredVelocity = moveDirection * moveSpeed; 
        Vector3 currentVelocity = rb.linearVelocity; 
        Vector3 velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z); 
        rb.AddForce(velocityChange, ForceMode.VelocityChange); 
        if (Input.GetKeyDown(jumpKey) && jumpLeft > 0) Jump(); 
    } 

    void Jump() 
    { 
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        jumpLeft--;
        if (targetHeight == crouchHeight)
        {
            targetHeight = playerHeight;
        }
        
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
        slideTimer = slideDuration;

        // Capture direction: if moving, slide that way; if still, slide forward
        slideDirection = moveDirection.magnitude > 0.1f ? moveDirection : transform.forward;

        targetHeight = crouchHeight;

        // Optional: Add an initial burst of speed
        rb.AddForce(slideDirection * slideSpeed, ForceMode.Impulse);
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
        // Check if there is a ceiling above the player
        bool ceilingAbove = Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + 0.2f, ceilingMask);

        if (!ceilingAbove)
        {
            targetHeight = playerHeight;
            currentState = moveDirection.magnitude > 0.1f ? PlayerState.Run : PlayerState.Idle;
        }
        else
        {
            // Stuck under something, stay crouched
            currentState = PlayerState.Crouch;
            targetHeight = crouchHeight;
        }
        Debug.Log("TryStand called. Blocked = " + ceilingAbove);
        Debug.Log("Standing. targetHeight = " + targetHeight);
        Debug.DrawRay(transform.position + Vector3.up * (crouchHeight / 2f), Vector3.up * ((playerHeight - crouchHeight) + ceilingCheckDistance), ceilingAbove ? Color.red : Color.green);
    } 

    void SmoothCrouchHeight() 
    {
        capsule.height = Mathf.Lerp(capsule.height, targetHeight, Time.deltaTime * 10f);
    } 

    // ????????????????????????????????????????????? 
    // GROUND CHECK 
    // ?????????????????????????????????????????????
    void CheckGround() 
    { 
        Vector3 origin = transform.position + Vector3.up * 0.1f; 
        isGrounded = Physics.Raycast(origin, Vector3.down, playerHeight / 2 + 0.2f, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.1f)
        {
            jumpLeft = jumps; // Reset jumps when we touch the floor
        }
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

        if (Input.GetKey(jumpKey) && isGrounded)
            Jump();
    }

    public void TakeDamage(int amount, Vector3 attackerPosition)
    {
        if (isHurt) return; // Prevent double-triggering

        maxLives -= amount;
        isHurt = true;
        currentState = PlayerState.Damage;
        damageTimer = damageStunDuration; // Ensure this is set to ~0.5f or higher

        // Trigger the actual physics push
        if (damageEffects != null)
        {
            damageEffects.ApplyKnockback(attackerPosition);
        }
    }
}