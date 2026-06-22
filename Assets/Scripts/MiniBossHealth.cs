using UnityEngine;

public class MiniBossHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 15;
    private int currentHealth;

    [Header("Parry")]
    public bool canParry = true;
    public float parryDuration = 0.5f;
    public float parryCooldown = 3f;
    [Header("Audio")]
    public AudioClip takeDamageSound;

    [Range(0f, 1f)]
    public float takeDamageVolume = 0.8f;

    [Header("Reward")]
    public bool giveCodeReward = true;
    public int codeDigitIndex = 2;
    public char codeDigit = '2';
    public string rewardMessage = "Получена цифра кода: **2*";

    [Header("Boss Health UI")]
    public BossHealthBarUI bossHealthBarUI;
    public bool showHealthBarOnStart = true;
    public string bossName = "Мини-босс";

    [Header("Animation")]
    public Animator animator;
    public float destroyDelay = 1f;

    private bool isParrying;
    private bool isDead;

    private Rigidbody2D rb;
    private Collider2D[] colliders;

    private MiniBossController miniBossController;
    private FlyingMiniBossController flyingMiniBossController;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponentsInChildren<Collider2D>();

        miniBossController = GetComponent<MiniBossController>();
        flyingMiniBossController = GetComponent<FlyingMiniBossController>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            currentHealth = DifficultyManager.Instance.GetMiniBossHealth(maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.bossName = bossName;

            if (showHealthBarOnStart)
            {
                bossHealthBarUI.UpdateBossHealth(currentHealth, currentHealth);
            }
            else
            {
                bossHealthBarUI.Hide();
            }
        }

        if (canParry)
        {
            StartCoroutine(ParryRoutine());
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        if (isParrying)
        {
            Debug.Log("MiniBoss parried player attack");
            return;
        }

        currentHealth -= damage;
        Vector3 sparkPosition = transform.position + Vector3.up * 0.8f;
        HitSparkEffect.Spawn(sparkPosition);
        PlayTakeDamageSound();

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("MiniBoss HP: " + currentHealth);

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void InstantDeath()
    {
        if (isDead) return;

        currentHealth = 0;
        UpdateHealthUI();
        Die();
    }

    public bool IsDead()
    {
        return isDead;
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    private void UpdateHealthUI()
    {
        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.bossName = bossName;
            bossHealthBarUI.UpdateBossHealth(currentHealth, GetDisplayedMaxHealth());
        }
    }

    private int GetDisplayedMaxHealth()
    {
        if (DifficultyManager.Instance != null)
        {
            return DifficultyManager.Instance.GetMiniBossHealth(maxHealth);
        }

        return maxHealth;
    }

    private System.Collections.IEnumerator ParryRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(parryCooldown);

            if (isDead) yield break;

            isParrying = true;

            if (animator != null)
            {
                animator.SetTrigger("Parry");
            }

            Debug.Log("MiniBoss parry active");

            yield return new WaitForSeconds(parryDuration);

            isParrying = false;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("MiniBoss died");

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (miniBossController != null)
        {
            miniBossController.enabled = false;
        }

        if (flyingMiniBossController != null)
        {
            flyingMiniBossController.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        foreach (Collider2D col in colliders)
        {
            if (col != null)
            {
                col.enabled = false;
            }
        }

        GiveReward();

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.Hide();
        }

        Destroy(gameObject, destroyDelay);
    }

    private void GiveReward()
    {
        if (!giveCodeReward) return;

        if (CodeProgress.Instance != null)
        {
            CodeProgress.Instance.SetKnownDigit(codeDigitIndex, codeDigit);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage(rewardMessage);
        }
    }
    private void PlayTakeDamageSound()
    {
        if (takeDamageSound == null) return;

        GameObject soundObject = new GameObject("MiniBossTakeDamageSound");
        soundObject.transform.position = transform.position;

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = takeDamageSound;
        audioSource.volume = takeDamageVolume;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        audioSource.Play();

        Destroy(soundObject, takeDamageSound.length + 0.1f);
    }
}