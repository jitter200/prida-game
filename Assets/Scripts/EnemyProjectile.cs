using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 8f;
    public float lifetime = 5f;

    [Header("Damage")]
    public int damage = 1;

    [Header("Rotation")]
    public bool rotateToDirection = true;

    // Если спрайт стрелы смотрит вправо — оставь 0
    // Если спрайт стрелы смотрит вверх — поставь -90
    // Если спрайт стрелы смотрит вниз — поставь 90
    public float rotationOffset = 0f;

    private Vector2 direction;

    public void Init(Vector2 shootDirection)
    {
        direction = shootDirection.normalized;

        if (rotateToDirection)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
        }

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = collision.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            int finalDamage = damage;

            if (DifficultyManager.Instance != null)
            {
                finalDamage = DifficultyManager.Instance.GetEnemyDamage(damage);
            }

            playerHealth.TakeDamage(finalDamage);
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}