using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Vector3 currentCheckPoint;

    private bool hasCheckpoint = false;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Set the checkpoint (e.g., NodeMap node or in-level checkpoint)
    public void SetCheckPoint(Vector3 newCheckPoint)
    {
        currentCheckPoint = newCheckPoint;
        hasCheckpoint = true;
        Debug.Log("[CHECKPOINT SET] " + currentCheckPoint);
    }

    // Respawn the player at the current checkpoint
    public void RespawnPlayer(Player player)
    {
        if (!hasCheckpoint)
        {
            Debug.LogWarning("Respawn attempted with no checkpoint set!");
            return;
        }

        if (player == null) return;

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb == null) return;

        // Stop physics
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true; // temporarily disable physics

        // Reset player
        player.ResetPlayer();

        // Teleport using Rigidbody
        rb.position = currentCheckPoint;
        rb.rotation = Quaternion.identity; // optional: reset rotation

        // Re-enable physics
        rb.isKinematic = false;

        Debug.Log("Respawning at: " + currentCheckPoint);
    }
}