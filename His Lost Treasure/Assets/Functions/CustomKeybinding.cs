using UnityEngine;
using UnityEngine.InputSystem;

public class CustomKeybinding : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("Player Actions")]
    private InputAction playerJump;
    private InputAction jumpAction;
    private InputAction jumpAction;
    private InputAction jumpAction;

    [Header("Map Actions")]
    private InputAction jumpAction;
    private InputAction jumpAction;

    [Header("UI Actions")]
    private InputAction jumpAction;
    private InputAction jumpAction;
    private InputAction jumpAction;

    [Header("Debug Actions")]
    private InputAction jumpAction;
    private InputAction jumpAction;

    void Awake()
    {
        playerJump = inputActionAsset.FindActionMap("Player").FindAction("Jump")
    }
    public void StartJumpRebind()
    {
        playerJump.Disable();
        playerJump.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            playerJump.Enable();
            Debug.Log("Jump binding complete!");
        }).Start();
    }
}
