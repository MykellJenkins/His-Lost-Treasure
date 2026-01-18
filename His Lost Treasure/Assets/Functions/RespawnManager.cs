using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public static RespawnManager Instance;

    private Vector3 currentCheckPoint;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCheckPoint(Vector3 newCheckPoint)
    {
        currentCheckPoint = newCheckPoint;
    }

    public void RespawnPlayer(GameObject player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false; // Disable the CharacterController to avoid collision issues
            player.transform.position = currentCheckPoint;
            controller.enabled = true; // Re-enable the CharacterController
        }
        else
        {
            // If no CharacterController is found, just set the position
            player.transform.position = currentCheckPoint;
        }
    }























}
