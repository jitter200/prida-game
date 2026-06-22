using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Parry")]
    public bool canParry = true;
    public float parryDuration = 0.4f;
    public float parryCooldown = 2f;

    [Header("Audio")]
    public AudioClip takeDamageSound;

    [Range(0f, 1f)]
    public float takeDamageVolume = 0.8f;

    [Header("Death")]
    public float deathDestroyDelay = 0.7f;

    [Header("Animation")]
    public Animator animator;

    private int currentHealth;
    private bool isParrying;
    private bool isDead;

    private Rigidbody2D rb;
    private Collider2D enemyCollider;
    private EnemyController enemyController;
    private EnemyShooter enemyShooter;
    private FlyingMeleeEnemy flyingMeleeEnemy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyCollider = GetComponent<Collider2D>();

        enemyController = GetComponent<EnemyController>();
        enemyShooter = GetComponent<EnemyShooter>();
        flyingMeleeEnemy = GetComponent<FlyingMeleeEnemy>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            currentHealth = DifficultyManager.Instance.GetEnemyHealth(maxHealth);
        }
        else
        {
            currentHealth = maxHealth;
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
            Debug.Log("Enemy parried player attack");
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Vector3 sparkPosition = transform.position + Vector3.up * 0.4f;
        HitSparkEffect.Spawn(sparkPosition);

        PlayTakeDamageSound();

        Debug.Log("Enemy HP: " + currentHealth);

        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void PlayTakeDamageSound()
    {
        if (takeDamageSound == null) return;

        GameObject soundObject = new GameObject("EnemyTakeDamageSound");
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

    private System.Collections.IEnumerator ParryRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(parryCooldown);

            if (isDead) yield break;

            isParrying = true;
            Debug.Log("Enemy parry active");

            yield return new WaitForSeconds(parryDuration);

            isParrying = false;
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        Debug.Log("Enemy died");

        if (animator != null)
        {
            animator.SetBool("IsDead", true);
        }

        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        if (enemyShooter != null)
        {
            enemyShooter.enabled = false;
        }

        if (flyingMeleeEnemy != null)
        {
            flyingMeleeEnemy.enabled = false;
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }

        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        Destroy(gameObject, deathDestroyDelay);
    }

    public void InstantDeath()
    {
        if (isDead) return;

        Die();
    }

    public bool IsParrying()
    {
        return isParrying;
    }

    public bool IsDead()
    {
        return isDead;
    }
}