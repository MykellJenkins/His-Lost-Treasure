using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class LedgeGrabRB : MonoBehaviour
{
    [Header("Ledge Detection")]
    public Transform ledgeCheckPoint;
    public float forwardCheckDistance = 0.6f;
    public float downCheckDistance = 1.5f;
    public LayerMask ledgeLayer;

    [Header("Hang Position")]
    public Vector3 hangOffset = new Vector3(0f, -1.2f, -0.4f);

    private Rigidbody rb;
    private bool isHanging;
    private Vector3 ledgePoint;
    private Vector3 ledgeNormal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!isHanging)
            TryGrabLedge();
        else
            HandleHangingInput();
    }

    void TryGrabLedge()
    {
        // Only grab while falling
        if (rb.linearVelocity.y > -0.1f) return;

        if (Physics.Raycast(
            ledgeCheckPoint.position,
            transform.forward,
            out RaycastHit wallHit,
            forwardCheckDistance,
            ledgeLayer))
        {
            Vector3 downCheckStart = wallHit.point + Vector3.up * 1.5f;

            if (Physics.Raycast(
                downCheckStart,
                Vector3.down,
                out RaycastHit ledgeHit,
                downCheckDistance,
                ledgeLayer))
            {
                StartHanging(ledgeHit.point, wallHit.normal);
            }
        }
    }

    void StartHanging(Vector3 grabPoint, Vector3 normal)
    {
        isHanging = true;
        ledgePoint = grabPoint;
        ledgeNormal = normal;

        // Stop all motion
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Freeze physics
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // Snap to hang position
        transform.position =
            ledgePoint +
            Vector3.up * hangOffset.y +
            ledgeNormal * hangOffset.z;
    }

    void HandleHangingInput()
    {
        if (Input.GetButtonDown("Jump"))
            ClimbUp();
        else if (Input.GetKeyDown(KeyCode.S))
            DropDown();
    }

    void ClimbUp()
    {
        isHanging = false;

        // Restore physics
        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Move player on top of ledge
        transform.position = ledgePoint + Vector3.up * 0.6f;
    }

    void DropDown()
    {
        isHanging = false;

        rb.useGravity = true;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
}
