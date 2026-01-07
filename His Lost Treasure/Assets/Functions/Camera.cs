using UnityEngine;

public class Camera : MonoBehaviour
{
    // Mouse sensitivity
    [SerializeField] int Sentivity;
    // Vertical rotation limits
    [SerializeField] int LockVerticalMin, LockVerticalMax;
    //  Whether to invert the Y-axis for camera control
    [SerializeField] bool InvertY;

    // Current vertical rotation of the camera
    float CameraRotationX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Hide the cursor
        Cursor.visible = false;
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse X movement
        float mouseX = Input.GetAxisRaw("Mouse X") * Sentivity * Time.deltaTime;
        // Get mouse Y movement
        float mouseY = Input.GetAxisRaw("Mouse Y") * Sentivity * Time.deltaTime;

        // Adjust vertical rotation based on InvertY setting
        if (InvertY)
        {
            // Adjust the vertical rotation based on mouse movement
            CameraRotationX += mouseY;
        }
        else
        {
            // Adjust the vertical rotation based on mouse movement
            CameraRotationX -= mouseY;
        }

        // Clamp the vertical rotation
        CameraRotationX = Mathf.Clamp(CameraRotationX, LockVerticalMin, LockVerticalMax);

        // Rotate the camera vertically
        transform.localRotation = Quaternion.Euler(CameraRotationX, 0, 0);

        // Rotate the player object horizontally
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
