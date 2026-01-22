using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("Inventory UI")]
    public Transform contentParent;
    public GameObject itemPrefab;

    [Header("Equipment Slots")]
    public Image weaponSlot;
    public Image artifactSlot;
    public Image glovesSlot;


    private List<InventoryItem> inventoryItems = new List<InventoryItem>();

    void Start()
    {
        //Items
        AddItem(new InventoryItem
        {
            itemName = "?",
          itemType = ItemType.Equipment
       });




        // Test items
        for (int i = 1; i <= 25; i++)
        {
            AddItem(new InventoryItem { itemName = "Item " + i });
        }
    }

    public void AddItem(InventoryItem item)
    {
        inventoryItems.Add(item);
        CreateItemUI(item);
    }

    void CreateItemUI(InventoryItem item)
    {
        GameObject uiItem = Instantiate(itemPrefab, contentParent);

        uiItem.transform.Find("Icon").GetComponent<Image>().sprite = item.icon;
        uiItem.transform.Find("Item Name").GetComponent<Text>().text = item.itemName;


        uiItem.GetComponent<Button>().onClick.AddListener(() =>
        {
            EquipItem(item);
        });
    }

    void EquipItem(InventoryItem item)
    {
        switch (item.itemType)
        {
            case ItemType.Equipment:
                weaponSlot.sprite = item.icon;
                break;

            case ItemType.Consumable:
                weaponSlot.sprite = item.icon;
                break;

            case ItemType.Miscellaneous:
                weaponSlot.sprite = item.icon;
                break;
        }
    }
}