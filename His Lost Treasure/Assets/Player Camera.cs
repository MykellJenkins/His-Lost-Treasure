using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.UI.ToggleMouseLock.started += OnCursorPressed;
        inputActions.UI.ToggleMouseLock.canceled += OnCursorReleased;
        inputActions.UI.Enable();
    }

    private void OnDisable()
    {
        inputActions.UI.ToggleMouseLock.started -= OnCursorPressed;
        inputActions.UI.ToggleMouseLock.canceled -= OnCursorReleased;
        inputActions.UI.Disable();
    }

    private void Start()
    {
        LockCursor();
    }

    private void OnCursorPressed(InputAction.CallbackContext context)
    {
        UnlockCursor();
    }

    private void OnCursorReleased(InputAction.CallbackContext context)
    {
        LockCursor();
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
