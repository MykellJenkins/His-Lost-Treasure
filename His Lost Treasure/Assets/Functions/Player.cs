using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player : MonoBehaviour, IDamage
{
    // Cache the CharacterController component
    [SerializeField] CharacterController CharacterController;

    // Player hit points
    [SerializeField] int HP;

    [Header("Player Lives")]
    // Total number of lives
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    //player movement variables
    // Speed of the player
    [SerializeField] float Speed;
    // Sprint modifier
    [SerializeField] float SprintModifier;
    // Jump variables
    [SerializeField] int JumpSpeed;
    // Maximum number of jumps allowed
    [SerializeField] int MaxJumps;
    // Current number of jumps made
    [SerializeField] int Jumps;
    // Gravity affecting the player
    [SerializeField] int Gravity;

    // Movement variables
    Vector3 MoveDirection;
    // Player velocity
    Vector3 PlayerVelocity;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale = new Vector3(1, 1f, 1);

    // Current jump count
    int JumpCount;

    //Original HP
    int HPOrig;

    // Sprinting state
    bool sprinting;
    bool isSprinting;

    [Header("Sliding Settings")]
    [SerializeField] float slideSpeed = 20f;
    [SerializeField] float slideDuration = 0.75f;
    [SerializeField] float slideCooldown = 1f;
    private float slideTimer;
    private float cooldownTimer;
    private bool isSliding;
    private Vector3 slideDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handle timers
        if (slideTimer > 0) slideTimer -= Time.deltaTime;
        if (cooldownTimer > 0) cooldownTimer -= Time.deltaTime;

        // SLIDING CHECK: Must hold Crouch button to remain in a slide
        if (isSliding)
        {
            if (Input.GetButton("Crouch") && slideTimer > 0)
            {
                SlideMovement();
            }
            else
            {
                StopSlide();
            }
        }
        else
        {
            Movement();
            Sprint();
            Crouch();

            // TRIGGER SLIDE: Must be sprinting, grounded, and JUST pressed Crouch
            if (Input.GetButtonDown("Crouch") && sprinting && CharacterController.isGrounded && cooldownTimer <= 0)
            {
                StartSlide();
            }
        }



        //Code for the players hearts
        // if HP is full it will show full hearts
        if (HP >= 3)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(true);

        }
        // if HP is 2 hearts it will show 2 full hearts instead of 3.
        else if (HP == 2)
        {
            heart1.SetActive(true);
            heart2.SetActive(true);
            heart3.SetActive(false);
        }
        else if (HP == 1)
        {
            heart1.SetActive(true);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }
        else if (HP <= 0)
        {
            heart1.SetActive(false);
            heart2.SetActive(false);
            heart3.SetActive(false);
        }



    }

    void Movement()
    {

        // Check if the player is grounded
        if (CharacterController.isGrounded)
        {
            // Reset vertical velocity when grounded
            PlayerVelocity = Vector3.zero;
            // Reset jump count when grounded
            JumpCount = 0;
        }
        else
        {

            // Apply gravity when not grounded
            PlayerVelocity.y -= Gravity * Time.deltaTime;
        }

        // Get input for movement direction
        MoveDirection = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        // Move the player
        CharacterController.Move(MoveDirection * Speed * Time.deltaTime);

        // Handle jumping
        Jump();

        // Apply vertical velocity
        CharacterController.Move(PlayerVelocity * Time.deltaTime);
    }

    void Sprint()
    {
        // Check if the sprint button is pressed
        if (Input.GetButtonDown("Sprint"))
        {
            isSprinting = true;
            sprinting = true;
            // Increase speed when sprint button is pressed
            Speed *= SprintModifier;

        }
        else if (Input.GetButtonUp("Sprint"))
        {
            isSprinting = false;
            sprinting = false;
            // Reset speed when sprint button is released
            Speed /= SprintModifier;

        }
    }
    void Jump()
    {
        // Check if the jump button is pressed and if the player can jump
        if (Input.GetButtonDown("Jump") && JumpCount < MaxJumps)
        {
            // Apply jump speed to the player's vertical velocity
            PlayerVelocity.y = JumpSpeed;
            // Increment jump count
            JumpCount++;
        }
    }

    void Crouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            transform.localScale = crouchScale;
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }
        if (Input.GetButtonUp("Crouch"))
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
    }

    void StartSlide()
    {
        isSliding = true;
        slideTimer = slideDuration;

        // Capture direction: Use current movement or face forward if still
        slideDirection = MoveDirection.normalized;
        if (slideDirection == Vector3.zero) slideDirection = transform.forward;

        // Ensure scale is set for the slide
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
    }

    void SlideMovement()
    {
        // Apply velocity in the locked slide direction
        CharacterController.Move(slideDirection * slideSpeed * Time.deltaTime);

        // Still apply gravity while sliding
        if (!CharacterController.isGrounded)
        {
            PlayerVelocity.y -= Gravity * Time.deltaTime;
            CharacterController.Move(PlayerVelocity * Time.deltaTime);
        }
    }

    void StopSlide()
    {
        isSliding = false;
        cooldownTimer = slideCooldown;
        slideTimer = 0;

        // IMPORTANT: If they let go of crouch, reset scale. 
        // If they are still holding crouch, keep the crouch scale.
        if (!Input.GetButton("Crouch"))
        {
            transform.localScale = playerScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
    }


    //Player Take Damage
    public void TakeDamage(int damageAmount)
    {
        HP -= damageAmount;
    }





}
