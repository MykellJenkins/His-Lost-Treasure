using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyID;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // Find all doors in the scene
        Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);

        foreach (Door door in doors)
        {
            if (door.requiredKeyID == keyID)
            {
                door.Unlock();
            }
        }

        gameObject.SetActive(false); // Pick up key
    }
}