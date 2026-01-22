using UnityEngine;


public enum ItemType
{
    Consumable,
    Equipment,
    Quest,
    Miscellaneous
}





[System.Serializable]
public class InventoryItem 
{
  public string itemName;
    public Sprite icon;
    public ItemType itemType;
}
