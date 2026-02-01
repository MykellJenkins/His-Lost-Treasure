using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

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
    [Header("Camera Controls")]
    public float mouseSensitivity = 4f;
    public bool invertY = false;

    // Components
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Transform cam;

    // State Machine
    public PlayerState currentState = PlayerState.Idle;

    // Lives
    public int maxLives = 3;
    public bool isHurt = false;
    //bool isMovingUp;

    // dmage effect
    public float damageStunDuration = 2f;
    public float damageTimer = 10;
    private PlayerDamageEffects damageEffects;
    private float invincibilityDuration = 0;
    public float invincibilityTimeAfterDamage = 2f;

    // Movement
    private PlayerInputActions inputActions;

    // cached inputs
    private Vector2 moveInput;
    private Vector2 lookInput;
    private bool jumpPressed;
    private bool sprintHeld;
    private bool crouchHeld;

    public float moveSpeed = 5f;
    private Vector3 moveDirection;
    public KeyCode forwardKey = KeyCode.W;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode backKey = KeyCode.S;
    public KeyCode rightKey = KeyCode.D;
    Vector3 currentVelocity;
    Vector3 velocityChange;
    Vector3 desiredVelocity;
    Vector3 horizontalvelocity;
    Vector3 verticalvelocity;

    // Sprinting
    public float SprintSpeed = 6f;
    bool IsSprinting;

    // Jumping
    public float jumpForce = 10f;
    public int jumpLeft = 2;
    public int jumps = 2;
    public float fallMultiplier = 2.5f;
    public float ascendMultiplier = 2f;

    // Crouch & Slide
    public float crouchHeight = 1f;
    public float crouchSpeed = 5f;
    public float slideSpeed = 15f;
    public float slideDuration = 1f;
    private float slideTimer;
    private Vector3 slideDirection;
    public float slideMinSpeed = 4f;
    float slideCooldown = 0.3f;
    float lastSlideTime = -10f;

    //Ground & Ceiling
    public LayerMask ignoreLayer;
    public LayerMask groundLayer;
    public LayerMask ceilingMask;
    public float ceilingCheckDistance = 0.1f;
    private bool isGrounded;
    bool crouchPressed;
    private float playerHeight;
    private float targetHeight;

    //movingplatform 
    Rigidbody movingPlatformRB;


    //damage flash
    public float flashInterval = 0.1f;

    private Renderer[] renderers;
    private Coroutine flashCoroutine;


    public float jumpBufferTime = 0.15f;
    public float coyoteTime = 0.12f;
    public float crouchBufferTime = 0.15f;

    float jumpBufferCounter;
    float coyoteCounter;
    float crouchBufferCounter;

    public float slopeForce = 25f;
    public float uphillDrag = 20f;
    public float slideGravity = 30f;
    public float maxSlideSpeed = 20f;
    public float groundStickForce = 10f;
    public float slideExitBoost = 1.05f;
    public float maxCarrySpeed = 10f;
    public float momentumDecay = 6f;
    
    Vector3 cachedSlideVelocity;
    RaycastHit slopeHit;
    float slopeAngle;
    Vector3 slopeNormal;

    // ????????????????????????????????????????????? 
    // UNITY METHODS 
    // ?????????????????????????????????????????????
    void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    void OnEnable()
    {
        currentState = PlayerState.Idle;
        isHurt = false;
        invincibilityDuration = 0;
        inputActions.Player.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += _ => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += _ => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += _ => 
        {
            jumpPressed = true;
            jumpBufferCounter = jumpBufferTime;
        };

        inputActions.Player.Sprint.started += _ => sprintHeld = true;
        inputActions.Player.Sprint.canceled += _ => sprintHeld = false;

        inputActions.Player.Crouch.started += _ =>
        {
            crouchHeld = true;
            crouchPressed = true;
            crouchBufferCounter = crouchBufferTime;
        };

        inputActions.Player.Crouch.canceled += _ => crouchHeld = false;

    }

    void Start()
    {
        // 1. Setup components
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        capsule = GetComponent<CapsuleCollider>();
        playerHeight = capsule.height;
        targetHeight = playerHeight;
        if (cam == null) cam = Camera.main.transform;
        damageEffects = GetComponent<PlayerDamageEffects>();
        renderers = GetComponentsInChildren<Renderer>();

        // 2. Load Global Settings (Volume/Sensitivity)
        LoadPlayerControls();

    }

    void Update()
    {
        CheckGround();
        ReadMovementInput();
        HandleStateTransitions();
        SmoothCrouchHeight();

        if (invincibilityDuration > 0)
        {
            invincibilityDuration -= Time.deltaTime;
        }

        jumpBufferCounter -= Time.deltaTime;
        coyoteCounter -= Time.deltaTime;
        crouchBufferCounter -= Time.deltaTime;
        crouchPressed = false;
        //if (rb.linearVelocity == transform.up)
        //{
        //    isMovingUp = true;
        //}
        //else { isMovingUp = false; }
        //if (invincibilityDuration > 0)
        //{
        //    invincibilityDuration -= Time.deltaTime;
        //}
    }

    void LateUpdate()
    {
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity * (invertY ? 1 : -1);

        transform.Rotate(Vector3.up * mouseX);

        Vector3 camRotation = cam.localEulerAngles;
        camRotation.x += mouseY;

        if (camRotation.x > 180f)
            camRotation.x -= 360f;

        camRotation.x = Mathf.Clamp(camRotation.x, -80f, 80f);

        cam.localEulerAngles = camRotation;
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
        if (SceneManager.GetActiveScene().name == "NodeMap")
            return;

        float horizontal = moveInput.x;
        float vertical = moveInput.y;

        Vector3 camForward = cam.forward;
        Vector3 camRight = cam.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * vertical) + (camRight * horizontal);

        if (moveDirection.magnitude > 1f)
            moveDirection.Normalize();

        if (moveDirection != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(
                transform.forward,
                moveDirection,
                Time.deltaTime * 10f
            );
        }
    }


    // ????????????????????????????????????????????? 
    // STATE TRANSITIONS 
    // ?????????????????????????????????????????????
    void HandleStateTransitions()
    {
        if (jumpBufferCounter > 0 && coyoteCounter > 0 && jumpLeft > 0)
        {
            jumpBufferCounter = 0;
            coyoteCounter = 0;
            currentState = PlayerState.Jump;
            Jump();
            return;
        }


        switch (currentState)
        {

            case PlayerState.Idle:

                // ENTER Run when player starts moving
                if (moveDirection.magnitude > 0.1f && isGrounded)
                {
                    currentState = PlayerState.Run;
                    break;
                }

                if (isGrounded && crouchBufferCounter > 0)
                {
                    crouchBufferCounter = 0;
                    currentState = PlayerState.Crouch;
                    EnterCrouch();
                    break;
                }

                if (isGrounded && crouchHeld)
                {
                    currentState = PlayerState.Crouch;
                    EnterCrouch();
                    break;
                }

                if (isHurt)
                    currentState = PlayerState.Damage;

                break;

            case PlayerState.Run:
                if (moveDirection.magnitude <= 0.1f) currentState = PlayerState.Idle;

                // Transition TO Sprint
                if (sprintHeld && isGrounded)
                    currentState = PlayerState.Sprint;
                if (isHurt == true) currentState = PlayerState.Damage;

                if (isGrounded && crouchBufferCounter > 0)
                {
                    crouchBufferCounter = 0;

                    if (rb.linearVelocity.magnitude > slideMinSpeed)
                        StartSlideState();
                    else
                    {
                        currentState = PlayerState.Crouch;
                        EnterCrouch();
                    }
                }
                break;

            case PlayerState.Sprint:

                // EXIT sprint immediately when Shift is released
                if (!sprintHeld)
                {
                    currentState = moveDirection.magnitude > 0.1f
                        ? PlayerState.Run
                        : PlayerState.Idle;
                    break;
                }

                // Stop sprint if player stops moving
                if (moveDirection.magnitude <= 0.1f)
                {
                    currentState = PlayerState.Idle;
                    break;
                }

                // Damage interrupt
                if (isHurt)
                {
                    currentState = PlayerState.Damage;
                    break;
                }

 
                if (isGrounded && crouchHeld)
                {
                    StartSlideState();
                    break;
                }

                break;

            case PlayerState.Jump:
                if (isGrounded)
                    currentState = moveDirection.magnitude > 0.1f ? PlayerState.Run : PlayerState.Idle;
                break;

            case PlayerState.Crouch:

                if (!crouchHeld)
                {
                    TryStand();
                    break;
                }

                if (crouchPressed && rb.linearVelocity.magnitude > slideMinSpeed)
                {
                    crouchPressed = false;
                    StartSlideState();
                    break;
                }

                // stay crouched, no repeated calls
                break;

            case PlayerState.Slide:
                slideTimer -= Time.deltaTime;
                if (slideTimer <= 0 || !crouchHeld || isHurt)
                {
                    ExitSlide();
                }
            break;

            case PlayerState.Damage:
                while (invincibilityDuration > 0f)
                    damageTimer -= Time.deltaTime;

                if (damageTimer <= 0f)
                {
                    isHurt = false;
                    currentState = PlayerState.Idle;

                    if (flashCoroutine != null)
                    {
                        StopCoroutine(flashCoroutine);
                        flashCoroutine = null;
                    }

                    SetRenderersVisible(true);
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
        if (currentState != PlayerState.Slide)
            rb.linearDamping = 0f;
        if (moveDirection.magnitude < 0.1f)
        {
            cachedSlideVelocity = Vector3.zero;
        }

        switch (currentState)
        {
            case PlayerState.Idle:
            case PlayerState.Run:
                Move(moveSpeed); // Pass the speed as a parameter
                IsSprinting = false;
            break;

            case PlayerState.Sprint:
                Move(SprintSpeed);
                IsSprinting = true;
            break;

            case PlayerState.Jump:
                Move(moveSpeed);
                break;

            case PlayerState.Crouch:
                Move(crouchSpeed);
            break;

            case PlayerState.Slide:
                rb.linearDamping = 1.5f;
            break;
        }
    }

    // ????????????????????????????????????????????? 
    // MOVEMENT HELPERS 
    // ?????????????????????????????????????????????
    void Move(float speed)
    {
        horizontalvelocity = Vector3.zero;
        if (isGrounded && movingPlatformRB != null)
        {
            horizontalvelocity = new Vector3(movingPlatformRB.linearVelocity.x, 0, movingPlatformRB.linearVelocity.z);
        }

        desiredVelocity = moveDirection * speed + horizontalvelocity;
        currentVelocity = rb.linearVelocity;

        velocityChange = desiredVelocity - new Vector3(currentVelocity.x, 0, currentVelocity.z);
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
       

        // Preserve slide momentum
        if (cachedSlideVelocity.magnitude > 0.1f)
        {
            Vector3 blend = Vector3.Lerp(
                cachedSlideVelocity,
                desiredVelocity,
                momentumDecay * Time.fixedDeltaTime
            );

            cachedSlideVelocity = blend;
            rb.linearVelocity = new Vector3(
                blend.x,
                currentVelocity.y,
                blend.z
            );
            return;
        }
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

        if (Time.time - lastSlideTime < slideCooldown)
            return;
        lastSlideTime = Time.time;
        currentState = PlayerState.Slide;
        slideTimer = slideDuration;

        // Capture direction: if moving, slide that way; if still, slide forward
        slideDirection = moveDirection.magnitude > 0.1f ? moveDirection : transform.forward;

        targetHeight = crouchHeight;

        // Optional: Add an initial burst of speed
        rb.AddForce(slideDirection * slideSpeed, ForceMode.Impulse);
    }

    void EnterCrouch()
    {
        targetHeight = crouchHeight;
    }

    void ExitSlide()
    {
        // Cache horizontal slide velocity
        cachedSlideVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);

        // Optional small boost
        cachedSlideVelocity *= slideExitBoost;

        if (cachedSlideVelocity.magnitude > maxCarrySpeed)
            cachedSlideVelocity = cachedSlideVelocity.normalized * maxCarrySpeed;

        if (crouchHeld)
        {
            currentState = PlayerState.Crouch;
            targetHeight = crouchHeight;
        }
        else
        {
            TryStand();
        }
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

    void ApplySlopeSlide()
    {
        // Project slide direction onto slope
        Vector3 slopeDir = Vector3.ProjectOnPlane(slideDirection, slopeNormal).normalized;

        // Downhill direction from gravity
        Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, slopeNormal).normalized;

        float downhillDot = Vector3.Dot(slopeDir, downhill);

        // Accelerate downhill, decelerate uphill
        if (downhillDot > 0)
            rb.AddForce(downhill * slopeForce, ForceMode.Acceleration);
        else
            rb.AddForce(-slopeDir * uphillDrag, ForceMode.Acceleration);

        // Allow some steering
        Vector3 steer = Vector3.ProjectOnPlane(moveDirection, slopeNormal) * 5f;
        rb.AddForce(steer, ForceMode.Acceleration);

        // Stick to ground
        rb.AddForce(-slopeNormal * groundStickForce, ForceMode.Acceleration);

        // Clamp max speed
        Vector3 flatVel = Vector3.ProjectOnPlane(rb.linearVelocity, Vector3.up);
        if (flatVel.magnitude > maxSlideSpeed)
        {
            rb.linearVelocity = flatVel.normalized * maxSlideSpeed + Vector3.up * rb.linearVelocity.y;
        }

        // End slide if too slow
        if (flatVel.magnitude < slideMinSpeed)
            ExitSlide();
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

        RaycastHit hit;

        if (Physics.Raycast(origin, Vector3.down, out hit, playerHeight / 2 + 0.2f, groundLayer))
        {
            isGrounded = true;
           coyoteCounter = coyoteTime; // Reset coyote time when grounded
            jumpLeft = jumps; // Reset jumps when we touch the floor
            if (hit.collider.CompareTag("MovingPlatform"))
            {
                movingPlatformRB = hit.rigidbody;
                
            }
            else
            {
                movingPlatformRB = null;

            }
        }
        else
        {
            
            isGrounded = false;
            movingPlatformRB = null;
        }
    }
  

    public void TakeDamage(int amount, Vector3 attackerPosition)
    {
        damageTimer = damageStunDuration;
        // Prevent repeated damage during invincibility
        if (invincibilityDuration > 0f) return;

        // Reduce health
        maxLives -= amount;
        Debug.Log($"Player took {amount} damage! Lives left: {maxLives}");

        // Trigger damage state/effects
        currentState = PlayerState.Damage;
        isHurt = true;
        invincibilityDuration = invincibilityTimeAfterDamage;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());

        // Reset crouch height if needed
        if (targetHeight == crouchHeight)
            targetHeight = playerHeight;

        // **RESPAWN AT CHECKPOINT**
        if (RespawnManager.Instance != null)
        {
            Debug.Log("Respawning at last checkpoint!");
            RespawnManager.Instance.RespawnPlayer(this);
        }

        // Optional: handle death
        if (maxLives <= 0)
        {
            GameManager.Instance.StateLose();
        }
    }

    IEnumerator FlashRoutine()
    {
        bool visible = true;

        while (damageTimer > 0)
        {
            visible = !visible;
            SetRenderersVisible(visible);

            yield return new WaitForSeconds(flashInterval);
        }

        // Ensure visible at end
        SetRenderersVisible(true);
    }

    void SetRenderersVisible(bool visible)
    {
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    public void ResetPlayer()
    {
        currentState = PlayerState.Idle;
        isHurt = false;
        invincibilityDuration = 0;
        damageTimer = 0;
        jumpLeft = jumps;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = false;
        }

        enabled = true; // ensure Player script is active
    }

    public PlayerSaveData GetSaveData()
    {
        // Pass 'this' (the player), the current transform, and the build index
        return new PlayerSaveData(
            maxLives,
            transform,
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    // Call this from Start() or GameManager when data is retrieved
    public void LoadFromSave(PlayerSaveData data)
    {
        if (data == null) return;

        this.maxLives = data.maxLives;


        this.transform.position = data.position.ToVector3();


    }

    void LoadPlayerControls()
    {
        MenuSaveData settings = SavePlayerData.Instance.LoadMenu();
        if (settings != null)
        {
            this.mouseSensitivity = settings.mouseSensitivity;
            this.invertY = settings.invertY;
            // Apply sensitivity to your Cinemachine or Camera script here
        }
    }


    void OnApplicationQuit()
    {
        // Using the Singleton to save the current snapshot of player data
        SavePlayerData.Instance.SavePlayer(GetSaveData());
    }

    void OnDisable()
    {
        SavePlayerData.Instance.SavePlayer(GetSaveData());
        inputActions.Player.Disable();
    }
}