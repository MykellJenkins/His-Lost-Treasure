using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    public Image icon;
    public Text countText;

    public void Setup(InventoryItem item, int count)
    {
        icon.sprite = item.icon;
        countText.text = count > 1 ? count.ToString() : "";
    }
}
