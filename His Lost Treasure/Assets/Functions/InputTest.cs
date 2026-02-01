using UnityEngine;
using UnityEngine.InputSystem;

public class InputTest : MonoBehaviour
{
    void Update()
    {
        if (Keyboard.current == null)
        {
            Debug.Log("Keyboard is NULL");
            return;
        }

        if (Keyboard.current.pKey.wasPressedThisFrame)
            Debug.Log("P detected");

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Debug.Log("Escape detected");
    }
}