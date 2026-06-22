using UnityEngine;

public class PendulumDamage : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;
    public float damageCooldown = 1f;

    [Header("Knockback")]
    public float knockbackForceX = 12f;
    public float knockbackForceY = 5f;
    public float knockbackDuration = 0.2f;

    private float lastDamageTime = -999f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TryDamagePlayer(collision);
    }

    private void TryDamagePlayer(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        if (Time.time < lastDamageTime + damageCooldown) return;

        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            int finalDamage = damage;

            if (DifficultyManager.Instance != null)
            {
                finalDamage = DifficultyManager.Instance.GetTrapDamage(damage);
            }

            playerHealth.TakeDamage(finalDamage);
        }

        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController != null)
        {
            Vector2 direction = collision.transform.position - transform.position;
            direction.Normalize();

            Vector2 knockback = new Vector2(
                direction.x * knockbackForceX,
                knockbackForceY
            );

            playerController.ApplyKnockback(knockback, knockbackDuration);
        }

        lastDamageTime = Time.time;
    }
}