using UnityEngine;
using UnityEngine.UI;

public class GameInventoryUI : MonoBehaviour
{
    public static GameInventoryUI Instance;

    public Text coinText;
    public Image reserveIcon;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateCoins(int amount)
    {
        coinText.text = amount.ToString();
    }

    public void UpdateReserve(GameItem item)
    {
        if (item == null)
        {
            reserveIcon.enabled = false;
        }
        else
        {
            reserveIcon.enabled = true;
            reserveIcon.sprite = item.icon;
        }
    }
}
