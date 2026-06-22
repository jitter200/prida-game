using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("UI")]
    public Image fillImage;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI hpText;

    [Header("Settings")]
    public string bossName = "Final Boss";

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        if (panel != null)
        {
            panel.SetActive(true);
        }

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
    }

    public void Hide()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void UpdateBossHealth(int currentHealth, int maxHealth)
    {
        Show();

        if (maxHealth <= 0) return;

        float fillAmount = currentHealth / (float)maxHealth;

        if (fillImage != null)
        {
            fillImage.fillAmount = fillAmount;
        }

        if (hpText != null)
        {
            hpText.text = currentHealth + " / " + maxHealth;
        }
    }
}