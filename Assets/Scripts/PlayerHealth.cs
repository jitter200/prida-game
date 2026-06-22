using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("UI")]
    public TextMeshProUGUI hpText;
    public HPBarSpriteUI hpBarUI;

    [Header("Parry")]
    public float parryWindow = 0.25f;
    public float parryCooldown = 1f;

    private bool isParrying;
    private bool canParry = true;

    private void Start()
    {
        if (PlayerStats.Instance != null)
        {
            maxHealth = PlayerStats.Instance.GetTotalMaxHealth(maxHealth);
        }

        currentHealth = maxHealth;

        UpdateUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && canParry)
        {
            StartCoroutine(Parry());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isParrying)
        {
            Debug.Log("Parried!");
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player died");

        PlayerRespawn playerRespawn = GetComponent<PlayerRespawn>();

        if (playerRespawn != null)
        {
            playerRespawn.Respawn();
        }

        currentHealth = maxHealth;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (hpText != null)
        {
            hpText.text = "HP: " + currentHealth;
        }

        if (hpBarUI != null)
        {
            hpBarUI.UpdateHP(currentHealth, maxHealth);
        }
    }

    private System.Collections.IEnumerator Parry()
    {
        canParry = false;
        isParrying = true;

        Debug.Log("Parry active");

        yield return new WaitForSeconds(parryWindow);

        isParrying = false;

        yield return new WaitForSeconds(parryCooldown);

        canParry = true;
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public void InstantDeath()
    {
        currentHealth = 0;
        UpdateUI();

        Die();
    }

    public void HealFull()
    {
        currentHealth = maxHealth;

        UpdateUI();

        Debug.Log("Player healed to full HP");
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateUI();

        Debug.Log("Player healed: " + amount);
    }
}