using UnityEngine;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("UI")]
    public GameObject shopPanel;

    [Header("Prices")]
    public int healPrice = 10;
    public int mapPrice = 20;
    public int bowPrice = 40;

    [Header("Map Hint")]
    public string mapHintText = "Подсказка: звезда, вода, стрела.";

    private PlayerHealth currentPlayerHealth;

    private void Awake()
    {
        Instance = this;

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void OpenShop(PlayerHealth playerHealth)
    {
        currentPlayerHealth = playerHealth;

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        Time.timeScale = 1f;
        currentPlayerHealth = null;
    }

    public void BuyHeal()
    {
        if (!TrySpendCoins(healPrice)) return;

        if (currentPlayerHealth != null)
        {
            currentPlayerHealth.HealFull();
        }

        ShowMessage("HP восстановлено");
    }

    public void BuyMap()
    {
        if (PlayerStats.Instance == null)
        {
            ShowMessage("PlayerStats не найден");
            return;
        }

        if (PlayerStats.Instance.hasMap)
        {
            ShowMessage("Карта уже куплена");
            return;
        }

        if (!TrySpendCoins(mapPrice)) return;

        PlayerStats.Instance.UnlockMap();

        ShowMessage("Карта куплена");
    }

    public void BuyBow()
    {
        if (PlayerLoadout.Instance == null)
        {
            ShowMessage("PlayerLoadout не найден");
            return;
        }

        if (PlayerLoadout.Instance.ownedWeapons.Contains("Bow"))
        {
            ShowMessage("Лук уже куплен");
            return;
        }

        if (!TrySpendCoins(bowPrice)) return;

        PlayerLoadout.Instance.AddWeapon("Bow");

        ShowMessage("Куплено новое оружие: Bow");

        if (RareRewardPopup.Instance != null)
        {
            RareRewardPopup.Instance.ShowWeaponPopup("Bow");
        }
    }

    private bool TrySpendCoins(int price)
    {
        if (PlayerStats.Instance == null)
        {
            ShowMessage("PlayerStats не найден");
            return false;
        }

        if (PlayerStats.Instance.coins < price)
        {
            ShowMessage("Недостаточно монет. Нужно: " + price);
            return false;
        }

        PlayerStats.Instance.coins -= price;

        Debug.Log("Coins left: " + PlayerStats.Instance.coins);

        return true;
    }

    private void ShowMessage(string message)
    {
        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage(message);
        }

        Debug.Log(message);
    }
}