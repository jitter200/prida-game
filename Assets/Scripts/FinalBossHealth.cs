using UnityEngine;

public class FinalBossHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int stage2StartPercent = 50;
    [Header("Audio")]
    public AudioClip takeDamageSound;

    [Range(0f, 1f)]
    public float takeDamageVolume = 0.8f;

    [Header("UI")]
    public BossHealthBarUI bossHealthBarUI;

    [Header("Final Choice")]
    public FinalBossChoiceUI finalChoiceUI;
    public GameObject objectToEnableOnKill;
    public GameObject objectToEnableOnSpare;
    public GameObject objectToEnableAfterAnyChoice;
    public GameObject objectToDisableAfterAnyChoice;

    [Header("References")]
    public FinalBossController bossController;

    [Header("Death")]
    public float destroyDelay = 0.2f;
    public bool destroyBossAfterKill = true;
    public bool disableBossAfterSpare = true;

    private int currentHealth;
    private bool stage2Started = false;
    private bool isDefeated = false;
    private bool choiceResolved = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (bossController == null)
        {
            bossController = GetComponent<FinalBossController>();
        }

        if (finalChoiceUI == null)
        {
            finalChoiceUI = FindObjectOfType<FinalBossChoiceUI>(true);
        }

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.UpdateBossHealth(currentHealth, maxHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDefeated) return;

        currentHealth -= damage;
        Vector3 sparkPosition = transform.position + Vector3.up * 1.0f;
        HitSparkEffect.Spawn(sparkPosition);
        PlayTakeDamageSound();

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.UpdateBossHealth(currentHealth, maxHealth);
        }

        Debug.Log("Final Boss HP: " + currentHealth + " / " + maxHealth);

        float hpPercent = (currentHealth / (float)maxHealth) * 100f;

        if (!stage2Started && hpPercent <= stage2StartPercent)
        {
            stage2Started = true;

            if (bossController != null)
            {
                bossController.EnterStage2();
            }

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Босс перешёл во вторую стадию");
            }
        }

        if (currentHealth <= 0)
        {
            DefeatBoss();
        }
    }

    private void DefeatBoss()
    {
        if (isDefeated) return;

        isDefeated = true;

        if (bossHealthBarUI != null)
        {
            bossHealthBarUI.Hide();
        }

        if (bossController != null)
        {
            bossController.Die();
        }

        DisableBossColliders();

        if (finalChoiceUI != null)
        {
            finalChoiceUI.Open(this);
        }
        else
        {
            ResolveFinalChoice(true);
        }
    }

    public void ResolveFinalChoice(bool killBoss)
    {
        if (choiceResolved) return;

        choiceResolved = true;

        Time.timeScale = 1f;

     
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.CompleteGame();
        }
        else
        {
            PlayerPrefs.SetInt("GameCompleted", 1);
            PlayerPrefs.Save();

            Debug.Log("Game completed saved directly through PlayerPrefs");
        }

        if (objectToEnableAfterAnyChoice != null)
        {
            objectToEnableAfterAnyChoice.SetActive(true);
        }

        if (objectToDisableAfterAnyChoice != null)
        {
            objectToDisableAfterAnyChoice.SetActive(false);
        }

        if (killBoss)
        {
            if (objectToEnableOnKill != null)
            {
                objectToEnableOnKill.SetActive(true);
            }

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Ты убил босса");
            }

            if (destroyBossAfterKill)
            {
                Destroy(gameObject, destroyDelay);
            }
        }
        else
        {
            if (objectToEnableOnSpare != null)
            {
                objectToEnableOnSpare.SetActive(true);
            }

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Ты пощадил босса");
            }

            if (disableBossAfterSpare)
            {
                DisableBossObject();
            }
        }
    }

    private void DisableBossColliders()
    {
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();

        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void DisableBossObject()
    {
        if (bossController != null)
        {
            bossController.enabled = false;
        }

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
    private void PlayTakeDamageSound()
    {
        if (takeDamageSound == null) return;

        GameObject soundObject = new GameObject("FinalBossTakeDamageSound");
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