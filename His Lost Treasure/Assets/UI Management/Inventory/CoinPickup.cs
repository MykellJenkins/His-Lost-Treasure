using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameInventory.Instance.AddCoin(1);
        Destroy(gameObject);
    }
}

