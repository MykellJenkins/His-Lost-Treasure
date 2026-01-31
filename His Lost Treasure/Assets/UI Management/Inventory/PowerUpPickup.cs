using UnityEngine;

public class PowerUpPickup : MonoBehaviour
{
    public GameItem item;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameInventory.Instance.AddItem(item);
        Destroy(gameObject);
    }
}
