using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite maxHealthSprite;
    public Sprite[] healthSprites;

    private int currentHealth;

    void start()
    {
        currentHealth = 3;
        spriteRenderer.sprite = maxHealthSprite;
    }

    public void SetHealth(int health)
    {
        currentHealth = health;
        spriteRenderer.sprite = healthSprites[health];
    }
}
