using UnityEngine;

public class GameInventory : MonoBehaviour
{
    public static GameInventory Instance;

    public int coins;
    public GameItem reserveItem;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddCoin(int amount = 1)
    {
        coins += amount;
        GameInventoryUI.Instance.UpdateCoins(coins);
    }

    public void SetReserve(GameItem item)
    {
        reserveItem = item;
        GameInventoryUI.Instance.UpdateReserve(item);
    }
    void Update()
    {
        if (reserveItem != null && Input.GetKeyDown(KeyCode.E))
        {
            UseReserveItem();
        }
    }

    public void UseReserveItem()
    {
        Debug.Log("Using item: " + reserveItem.itemName);

        // TODO: Add actual item effect here

        reserveItem = null;
        GameInventoryUI.Instance.UpdateReserve(null);
    }

}
