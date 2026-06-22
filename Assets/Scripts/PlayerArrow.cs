using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 12f;
    public float lifetime = 3f;

    private int damage;
    private Vector2 direction;

    public void Init(Vector2 shootDirection, int arrowDamage)
    {
        direction = shootDirection.normalized;
        damage = arrowDamage;

        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) return;

        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        MiniBossHealth miniBossHealth = collision.GetComponent<MiniBossHealth>();
        if (miniBossHealth != null)
        {
            miniBossHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        BreakableCage cage = collision.GetComponent<BreakableCage>();
        if (cage != null)
        {
            cage.Break();
            Destroy(gameObject);
            return;
        }

        PuzzleTorch torch = collision.GetComponent<PuzzleTorch>();
        if (torch != null)
        {
            torch.HitByPlayer();
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }
}