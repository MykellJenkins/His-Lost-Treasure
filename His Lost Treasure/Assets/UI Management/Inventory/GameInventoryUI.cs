using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameInventoryUI : MonoBehaviour
{
    public static GameInventoryUI Instance;

    public Text coinText;
    public Transform slotContainer;
    public GameObject itemSlotPrefab;

    public CanvasGroup canvasGroup;

    void Start()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (canvasGroup.alpha > 0)
            StartCoroutine(FadeOut());
        else
            StartCoroutine(FadeIn());
    }
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
    IEnumerator FadeIn()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 3;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t);
            yield return null;
        }
        canvasGroup.alpha = 1;
    }

    IEnumerator FadeOut()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 3;
            canvasGroup.alpha = Mathf.Lerp(1, 0, t);
            yield return null;
        }
        canvasGroup.alpha = 0;


    }
}



