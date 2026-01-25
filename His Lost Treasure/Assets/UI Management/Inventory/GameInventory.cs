using UnityEngine;
using System.Collections.Generic;

public class GameInventory : MonoBehaviour
{
    public static GameInventory Instance;

    public int coins;

    public List<GameItem> items = new List<GameItem>();
    public int selectedIndex = 0;

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

    public void AddItem(GameItem item)
    {
        items.Add(item);
        GameInventoryUI.Instance.UpdateItems(items, selectedIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseSelectedItem();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectPreviousItem();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SelectNextItem();
        }
    }

    public void SelectNextItem()
    {
        if (items.Count == 0) return;
        selectedIndex = (selectedIndex + 1) % items.Count;
        GameInventoryUI.Instance.UpdateItems(items, selectedIndex);
    }

    public void SelectPreviousItem()
    {
        if (items.Count == 0) return;
        selectedIndex--;
        if (selectedIndex < 0) selectedIndex = items.Count - 1;
        GameInventoryUI.Instance.UpdateItems(items, selectedIndex);
    }

    public void UseSelectedItem()
    {
        if (items.Count == 0) return;

        GameItem selectedItem = items[selectedIndex];
        Debug.Log("Using item: " + selectedItem.itemName);

        // TODO: Add actual item effect here

        items.RemoveAt(selectedIndex);

        if (selectedIndex >= items.Count)
            selectedIndex = items.Count - 1;

        GameInventoryUI.Instance.UpdateItems(items, selectedIndex);
    }

}
