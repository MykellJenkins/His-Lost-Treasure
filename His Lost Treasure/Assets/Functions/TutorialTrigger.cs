using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [TextArea(2, 5)]
    public string message; // Tutorial text for this zone

    public bool waitForPlayerInput = true; // Wait for Enter key to continue

    private bool triggered = false; // Avoid triggering multiple times

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return; // Only trigger once
        if (!other.CompareTag("Player")) return; // Only trigger for player

        triggered = true;

    }
}
