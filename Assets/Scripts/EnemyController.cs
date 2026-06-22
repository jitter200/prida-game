using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public bool movingRight = true;

    [Header("Checks")]
    public Transform wallCheck;
    public Transform groundCheck;
    public float checkRadius = 0.15f;
    public LayerMask groundLayer;
    public LayerMask turnAroundLayer;

    [Header("Flip")]
    public float flipCooldown = 0.2f;
    public bool turnAroundWhenTouchEnemy = true;

    [Header("Damage")]
    public int damage = 1;
    public float damageCooldown = 1f;

    [Header("Animation")]
    public Animator animator;
    public bool useAttackAnimationOnContact = true;

    private float lastDamageTime;
    private float lastFlipTime;

    private Rigidbody2D rb;
    private EnemyHealth enemyHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyHealth = GetComponent<EnemyHealth>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void FixedUpdate()
    {
        if (enemyHealth != null && enemyHealth.IsDead())
        {
            rb.velocity = Vector2.zero;
            SetSpeedAnimation(0f);
            return;
        }

        float direction = movingRight ? 1f : -1f;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);

        SetSpeedAnimation(Mathf.Abs(rb.velocity.x));

        bool touchingWall = false;
        bool hasGroundAhead = true;

        if (wallCheck != null)
        {
            touchingWall = Physics2D.OverlapCircle(
                wallCheck.position,
                checkRadius,
                groundLayer | turnAroundLayer
            );
        }

        if (groundCheck != null)
        {
            hasGroundAhead = Physics2D.OverlapCircle(
                groundCheck.position,
                checkRadius,
                groundLayer
            );
        }

        if ((touchingWall || !hasGroundAhead) && Time.time >= lastFlipTime + flipCooldown)
        {
            Flip();
        }
    }

    private void SetSpeedAnimation(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    private void Flip()
    {
        lastFlipTime = Time.time;

        movingRight = !movingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }
    public void FaceTargetX(float targetX)
    {
        bool shouldFaceRight = targetX > transform.position.x;

        if (shouldFaceRight == movingRight) return;
        if (Time.time < lastFlipTime + flipCooldown) return;

        Flip();
    }

    public bool IsFacingRight()
    {
        return movingRight;
    }
    private void TryTurnAroundFromEnemy(GameObject otherObject)
    {
        if (!turnAroundWhenTouchEnemy) return;
        if (!otherObject.CompareTag("Enemy")) return;
        if (Time.time < lastFlipTime + flipCooldown) return;

        Flip();

        Debug.Log("Enemy met another enemy and turned around");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryTurnAroundFromEnemy(collision.gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (enemyHealth != null && enemyHealth.IsDead()) return;

        // Если встретил другого врага — разворачивается
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TryTurnAroundFromEnemy(collision.gameObject);
            return;
        }

        // Урон наносим только игроку
        if (!collision.gameObject.CompareTag("Player")) return;

        if (Time.time < lastDamageTime + damageCooldown) return;

        if (animator != null && useAttackAnimationOnContact)
        {
            animator.SetTrigger("Attack");
        }

        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = collision.gameObject.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null) return;

        if (playerHealth.IsParrying())
        {
            Debug.Log("Enemy parried!");
            lastDamageTime = Time.time;
            return;
        }

        int finalDamage = damage;

        if (DifficultyManager.Instance != null)
        {
            finalDamage = DifficultyManager.Instance.GetEnemyDamage(damage);
        }

        playerHealth.TakeDamage(finalDamage);
        lastDamageTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        }
    }
}