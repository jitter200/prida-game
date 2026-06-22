using UnityEngine;
using UnityEngine.UI;

public class HPBarSpriteUI : MonoBehaviour
{
    [Header("UI")]
    public Image hpImage;

    [Header("HP Sprites")]
    public Sprite hp100Sprite;
    public Sprite hp75Sprite;
    public Sprite hp50Sprite;
    public Sprite hp25Sprite;
    public Sprite hp5Sprite;
    public Sprite hp0Sprite;

    public void UpdateHP(int currentHealth, int maxHealth)
    {
        if (hpImage == null) return;
        if (maxHealth <= 0) return;

        float hpPercent = (currentHealth / (float)maxHealth) * 100f;

        if (hpPercent >= 100f)
        {
            hpImage.sprite = hp100Sprite;
        }
        else if (hpPercent >= 75f)
        {
            hpImage.sprite = hp75Sprite;
        }
        else if (hpPercent >= 50f)
        {
            hpImage.sprite = hp50Sprite;
        }
        else if (hpPercent >= 25f)
        {
            hpImage.sprite = hp25Sprite;
        }
        else if (hpPercent >= 5f)
        {
            hpImage.sprite = hp5Sprite;
        }
        else
        {
            hpImage.sprite = hp0Sprite;
        }
    }
}