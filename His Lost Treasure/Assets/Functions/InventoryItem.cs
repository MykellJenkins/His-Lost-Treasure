using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemID;
    public string itemName;
    public Sprite icon;
    public bool stackable = true;
}
