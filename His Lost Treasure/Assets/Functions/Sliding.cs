using UnityEngine;

public class PlayerSliding : MonoBehaviour
{
    [Header("References")]
    public Transform playerCamera; // Camera for direction reference

    [Header("Sliding Settings")]
    public float slideForce = 10f;         // Force applied while sliding
    public float slideDuration = 1.5f;     // How long the slide lasts
    public float slideCooldown = 0.5f;     // Time before you can slide again
    public float crouchHeight = 0.5f;      // Height while sliding
    public float normalHeight = 2f;        // Normal standing height

    [Header("Slope Settings")]
    public float maxSlopeAngle = 50f;      // Max slope angle for sliding boost

    private Rigidbody rb;
    private CapsuleCollider capsule;
    private bool isSliding = false;
    private bool canSlide = true;
    private float slideTimer = 0f;

    private Vector3 slideDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // Prevent unwanted rotation
        capsule = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        // Start sliding when LeftControl is pressed and player is moving
        if (Input.GetKeyDown(KeyCode.LeftControl) && canSlide && rb.linearVelocity.magnitude > 1f)
        {
            StartSlide();
        }

        // Stop sliding when time is up or key is released
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0 || Input.GetKeyUp(KeyCode.LeftControl))
            {
                StopSlide();
            }
        }
    }

    void FixedUpdate()
    {
        if (isSliding)
        {
            // Apply force in slide direction
            rb.AddForce(slideDirection * slideForce, ForceMode.Acceleration);

            // Extra boost if on a slope
            if (OnSlope(out Vector3 slopeDir))
            {
                rb.AddForce(slopeDir * slideForce * 1.5f, ForceMode.Acceleration);
            }
        }
    }

    void StartSlide()
    {
        isSliding = true;
        canSlide = false;
        slideTimer = slideDuration;

        // Reduce player height
        capsule.height = crouchHeight;

        // Set slide direction based on camera forward
        slideDirection = playerCamera.forward;
        slideDirection.y = 0; // Keep it horizontal
        slideDirection.Normalize();

        // Small downward force to keep grounded
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // Reset cooldown after slide
        Invoke(nameof(ResetSlide), slideCooldown + slideDuration);
    }

    void StopSlide()
    {
        isSliding = false;
        capsule.height = normalHeight;
    }

    void ResetSlide()
    {
        canSlide = true;
    }

    bool OnSlope(out Vector3 slopeDirection)
    {
        slopeDirection = Vector3.zero;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, capsule.bounds.extents.y + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            if (angle > 0 && angle <= maxSlopeAngle)
            {
                slopeDirection = Vector3.ProjectOnPlane(slideDirection, hit.normal).normalized;
                return true;
            }
        }
        return false;
    }
}
