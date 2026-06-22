using UnityEngine;

public class FinalBossProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 5f;

    [Header("Default Movement")]
    public float defaultSpeed = 8f;
    public Vector2 defaultDirection = Vector2.left;

    private Vector2 direction;
    private float speed;
    private int damage;

    private bool initialized;

    private void Start()
    {
        if (!initialized)
        {
            Init(defaultDirection, defaultSpeed, damage);
        }
    }

    public void Init(Vector2 shootDirection, float projectileSpeed, int projectileDamage)
    {
        direction = shootDirection.normalized;

        if (direction == Vector2.zero)
        {
            direction = defaultDirection.normalized;
        }

        speed = projectileSpeed;
        damage = projectileDamage;

        initialized = true;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

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
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}