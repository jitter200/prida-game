using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UltimateCooldownBarUI : MonoBehaviour
{
    [Header("UI")]
    public Image ultimateImage;
    public TextMeshProUGUI cooldownText;

    [Header("Sprites")]
    public Sprite ultimate100Sprite;
    public Sprite ultimate75Sprite;
    public Sprite ultimate50Sprite;
    public Sprite ultimate25Sprite;
    public Sprite ultimate0Sprite;

    [Header("Ultimate References")]
    public Ultimate_Rewind rewindUltimate;
    public Ultimate_Shockwave shockwaveUltimate;
    public Ultimate_Heal healUltimate;
    public Ultimate_LightningStrike lightningUltimate;

    [Header("Fallback")]
    public string fallbackUltimateName = "Rewind";

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            if (rewindUltimate == null)
                rewindUltimate = player.GetComponent<Ultimate_Rewind>();

            if (shockwaveUltimate == null)
                shockwaveUltimate = player.GetComponent<Ultimate_Shockwave>();

            if (healUltimate == null)
                healUltimate = player.GetComponent<Ultimate_Heal>();

            if (lightningUltimate == null)
                lightningUltimate = player.GetComponent<Ultimate_LightningStrike>();
        }
    }

    private void Update()
    {
        string currentUltimate = fallbackUltimateName;

        if (PlayerLoadout.Instance != null)
        {
            currentUltimate = PlayerLoadout.Instance.currentUltimate;
        }

        float remaining = 0f;
        float cooldown = 1f;

        if (currentUltimate == "Rewind" && rewindUltimate != null)
        {
            remaining = rewindUltimate.GetCooldownRemaining();
            cooldown = rewindUltimate.GetCooldownDuration();
        }
        else if (currentUltimate == "Shockwave" && shockwaveUltimate != null)
        {
            remaining = shockwaveUltimate.GetCooldownRemaining();
            cooldown = shockwaveUltimate.GetCooldownDuration();
        }
        else if (currentUltimate == "Heal" && healUltimate != null)
        {
            remaining = healUltimate.GetCooldownRemaining();
            cooldown = healUltimate.GetCooldownDuration();
        }
        else if (currentUltimate == "Lightning" && lightningUltimate != null)
        {
            remaining = lightningUltimate.GetCooldownRemaining();
            cooldown = lightningUltimate.GetCooldownDuration();
        }

        UpdateUltimateBar(remaining, cooldown);
    }

    private void UpdateUltimateBar(float remaining, float cooldown)
    {
        if (ultimateImage == null) return;

        float readyPercent = 100f;

        if (cooldown > 0f)
        {
            readyPercent = (1f - Mathf.Clamp01(remaining / cooldown)) * 100f;
        }

        if (remaining <= 0.05f)
        {
            readyPercent = 100f;
        }

        if (readyPercent >= 100f)
        {
            ultimateImage.sprite = ultimate100Sprite;
        }
        else if (readyPercent >= 75f)
        {
            ultimateImage.sprite = ultimate75Sprite;
        }
        else if (readyPercent >= 40f)
        {
            ultimateImage.sprite = ultimate50Sprite;
        }
        else if (readyPercent >= 1f)
        {
            ultimateImage.sprite = ultimate25Sprite;
        }
        else
        {
            ultimateImage.sprite = ultimate0Sprite;
        }

        if (cooldownText != null)
        {
            if (remaining > 0f)
            {
                cooldownText.text = Mathf.CeilToInt(remaining) + " сек";
            }
            else
            {
                cooldownText.text = "Готово";
            }
        }
    }
}