using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Vector3 currentCheckPoint;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Set the checkpoint (e.g., NodeMap node or in-level checkpoint)
    public void SetCheckPoint(Vector3 newCheckPoint)
    {
        currentCheckPoint = newCheckPoint;
    }

    // Respawn the player at the current checkpoint
    public void RespawnPlayer(Player player)
    {
        if (player == null) return;

        CharacterController controller = player.GetComponent<CharacterController>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        // Disable physics to prevent collision issues
        if (controller != null) controller.enabled = false;
        if (rb != null) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }

        // Move player to checkpoint
        player.transform.position = currentCheckPoint;

        // Reset player state
        player.ResetPlayer();

        // Re-enable physics
        if (controller != null) controller.enabled = true;
    }
}