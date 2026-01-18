using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerHealthUI : MonoBehaviour
{
    public int maxHealth = 3; // Total number of hearts
    public int currentHealth; // Current health points

    // List to hold the heart UI images in order
    public List<Image> heartImages = new List<Image>(); 

    public Sprite fullHeartSprite;
    public Sprite emptyHeartSprite;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    // Call this method to modify the player's health
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth < 0) currentHealth = 0;
        UpdateHeartsUI();

        if (currentHealth <= 0)
        {
            // Handle player death (e.g., game over, restart level)
            Debug.Log("Player Died!");
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        UpdateHeartsUI();
    }

    // Updates the heart sprites in the UI based on current health
    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < currentHealth)
            {
                heartImages[i].sprite = fullHeartSprite;
            }
            else
            {
                heartImages[i].sprite = emptyHeartSprite;
            }
        }
    }
}
