using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    void Start()
    {
        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;

        // Hide the cursor
        Cursor.visible = false;
    }

    void Update()
    {
        // Unlock and show the cursor when Escape is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
