using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameInventoryUI : MonoBehaviour
{
    public static GameInventoryUI Instance;

    public Text coinText;
    public Transform slotContainer;
    public GameObject itemSlotPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateCoins(int amount)
    {
        coinText.text = amount.ToString();
    }

    public void UpdateItems(List<GameItem> items, int selectedIndex)
    {
        // Clear old
        foreach (Transform child in slotContainer)
            Destroy(child.gameObject);

        for (int i = 0; i < items.Count; i++)
        {
            GameObject slot = Instantiate(itemSlotPrefab, slotContainer);
            Image icon = slot.GetComponent<Image>();
            icon.sprite = items[i].icon;

            // Highlight selected
            slot.GetComponent<Image>().color = (i == selectedIndex) ? Color.yellow : Color.white;
        }
    }
}
