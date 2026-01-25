using UnityEngine;
using UnityEngine.InputSystem;

public class CustomKeybinding : MonoBehaviour
{
    [Header("Input Actions")]
    public InputActionAsset inputActionAsset;

    [Header("Player Actions")]
    private InputAction playerJump;
    private InputAction playerMove;
    private InputAction playerGrapple;
    private InputAction playerPause;

    [Header("Map Actions")]
    private InputAction mapNavigate;
    private InputAction mapConfirm;

    [Header("UI Actions")]
    private InputAction uiNavigate;
    private InputAction uiConfirm;
    private InputAction uiCancel;

    [Header("Debug Actions")]
    private InputAction debugMenu;
    private InputAction debugConfirm;

    void Awake()
    {
        playerJump = inputActionAsset.FindActionMap("Player").FindAction("Jump");
        playerMove = inputActionAsset.FindActionMap("Player").FindAction("Move");
        playerGrapple = inputActionAsset.FindActionMap("Player").FindAction("Grapple");
        playerPause = inputActionAsset.FindActionMap("Player").FindAction("Pause");

        mapNavigate = inputActionAsset.FindActionMap("Map").FindAction("Navigate");
        mapConfirm = inputActionAsset.FindActionMap("Map").FindAction("Confirm");

        uiNavigate = inputActionAsset.FindActionMap("UI").FindAction("Navigate");
        uiConfirm = inputActionAsset.FindActionMap("UI").FindAction("Confirm");
        uiCancel = inputActionAsset.FindActionMap("UI").FindAction("Cancel");

        debugMenu = inputActionAsset.FindActionMap("Debug").FindAction("Menu");
        debugConfirm = inputActionAsset.FindActionMap("Debug").FindAction("Confirm");

    }
    public void StartPlayerJumpRebind()
    {
        playerJump.Disable();
        playerJump.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            playerJump.Enable();
            Debug.Log("Player jump binding complete!");
        }).Start();
    }
    public void StartPlayerMoveRebind()
    {
        playerMove.Disable();
        StartPlayerMoveUpRebind();
    }
    public void StartPlayerMoveUpRebind()
    {
        playerMove.PerformInteractiveRebinding(1).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("Player move up binding complete!");
            StartPlayerMoveDownRebind();
        }).Start();
        
    }
    private void StartPlayerMoveDownRebind()
    {
        playerMove.PerformInteractiveRebinding(2).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("Player move down binding complete!");
            StartPlayerMoveLeftRebind();
        }).Start();
        
    }
    private void StartPlayerMoveLeftRebind()
    {
        playerMove.PerformInteractiveRebinding(3).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("Player move left binding complete!");
            StartPlayerMoveRightRebind();
        }).Start();
       
    }
    private void StartPlayerMoveRightRebind()
    {
        playerMove.PerformInteractiveRebinding(4).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            playerMove.Enable();
            Debug.Log("Player move right binding complete!");
        }).Start();
    }

    public void StartPlayerGrappleRebind()
    {
        playerGrapple.Disable();
        playerGrapple.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            playerGrapple.Enable();
            Debug.Log("Player grapple binding complete!");
        }).Start();
    }
    public void StartPlayerPauseRebind()
    {
        playerPause.Disable();
        playerPause.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            playerPause.Enable();
            Debug.Log("Player pause binding complete!");
        }).Start();
    }

    public void StartMapNavigateRebind()
    {
        mapNavigate.Disable();
        StartMapNavigateLeftRebind();
    }
    private void StartMapNavigateLeftRebind()
    {
        mapNavigate.PerformInteractiveRebinding(3).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("Map navigate left binding complete!");
            StartMapNavigateRightRebind();
        }).Start();
    }
    private void StartMapNavigateRightRebind()
    {
        mapNavigate.PerformInteractiveRebinding(4).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            mapNavigate.Enable();
            Debug.Log("Map navigate right binding complete!");
        }).Start();
    }
    public void StartMapConfirmRebind()
    {
        mapConfirm.Disable();
        mapConfirm.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            mapConfirm.Enable();
            Debug.Log("Map confirm binding complete!");
        }).Start();
    }

    public void StartUINavigateRebind()
    {
        uiNavigate.Disable();
        StartUINavigateUpRebind();
    }
    private void StartUINavigateUpRebind()
    {
        uiNavigate.PerformInteractiveRebinding(1).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("UI navigate up binding complete!");
            StartUINavigateDownRebind();
        }).Start();
    }
    private void StartUINavigateDownRebind()
    {
        uiNavigate.PerformInteractiveRebinding(2).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("UI navigate down binding complete!");
            StartUINavigateLeftRebind();
        }).Start();
    }
    private void StartUINavigateLeftRebind()
    {
        uiNavigate.PerformInteractiveRebinding(3).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            Debug.Log("UI navigate left binding complete!");
            StartUINavigateRightRebind();
        }).Start();
    }
    private void StartUINavigateRightRebind()
    {
        uiNavigate.PerformInteractiveRebinding(4).WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            uiNavigate.Enable();
            Debug.Log("UI navigate right binding complete!");
        }).Start();
    }
    public void StartUIConfirmRebind()
    {
        uiConfirm.Disable();
        uiConfirm.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            uiConfirm.Enable();
            Debug.Log("UI confirm binding complete!");
        }).Start();
    }
    public void StartUICancelRebind()
    {
        uiCancel.Disable();
        uiCancel.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            uiCancel.Enable();
            Debug.Log("UI cancel binding complete!");
        }).Start();
    }

    public void StartDebugMenuRebind()
    {
        debugMenu.Disable();
        debugMenu.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            debugMenu.Enable();
            Debug.Log("Debug menu binding complete!");
        }).Start();
    }
    public void StartDebugConfirmRebind()
    {
        debugConfirm.Disable();
        debugConfirm.PerformInteractiveRebinding().WithControlsExcluding("Mouse").OnComplete(operation =>
        {
            operation.Dispose();
            debugConfirm.Enable();
            Debug.Log("Debug confirm binding complete!");
        }).Start();
    }
}
