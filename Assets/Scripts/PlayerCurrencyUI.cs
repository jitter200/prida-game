using TMPro;
using UnityEngine;

public class PlayerCurrencyUI : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI keysText;

    [Header("Format")]
    public string coinsPrefix = "Coins: ";
    public string keysPrefix = "Keys: ";

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (PlayerStats.Instance == null)
        {
            if (coinsText != null)
            {
                coinsText.text = coinsPrefix + "0";
            }

            if (keysText != null)
            {
                keysText.text = keysPrefix + "0";
            }

            return;
        }

        if (coinsText != null)
        {
            coinsText.text = coinsPrefix + PlayerStats.Instance.coins;
        }

        if (keysText != null)
        {
            keysText.text = keysPrefix + PlayerStats.Instance.keys;
        }
    }
}