using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;

    [Header("Stats")]
    public int coins = 0;
    public int bonusDamage = 0;
    public float bonusSpeed = 0f;
    public int bonusMaxHealth = 0;
    public int keys = 0;

    [Header("Items")]
    public bool hasMap = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
   
    public int GetTotalDamage(int baseDamage)
    {
        return baseDamage + bonusDamage;
    }

    public float GetTotalSpeed(float baseSpeed)
    {
        return baseSpeed + bonusSpeed;
    }

    public int GetTotalMaxHealth(int baseMaxHealth)
    {
        return baseMaxHealth + bonusMaxHealth;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public void AddDamage(int amount)
    {
        bonusDamage += amount;
        Debug.Log("Bonus damage: " + bonusDamage);
    }

    public void AddSpeed(float amount)
    {
        bonusSpeed += amount;
        Debug.Log("Bonus speed: " + bonusSpeed);
    }

    public void AddMaxHealth(int amount)
    {
        bonusMaxHealth += amount;
        Debug.Log("Bonus max health: " + bonusMaxHealth);
    }
    public void AddKey(int amount)
    {
        keys += amount;
        Debug.Log("Keys: " + keys);
    }

    public bool HasKeys(int amount)
    {
        return keys >= amount;
    }

    public void SpendKeys(int amount)
    {
        keys -= amount;

        if (keys < 0)
        {
            keys = 0;
        }

        Debug.Log("Keys left: " + keys);
    }
    public void UnlockMap()
    {
        hasMap = true;
        Debug.Log("Map unlocked");
    }
}