using UnityEngine;

public class FlyingMeleeEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 0.8f;

    [Header("Enemy Avoidance")]
    public LayerMask enemyLayer;
    public float turnAwayDuration = 0.35f;
    public float turnAwayCooldown = 0.4f;

    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1f;
    public float attackRange = 1f;

    [Header("Knockback")]
    public float knockbackForce = 7f;
    public float knockbackDuration = 0.15f;

    [Header("Animation")]
    public Animator animator;

    private Transform target;
    private Rigidbody2D rb;
    private float lastAttackTime = -999f;

    private bool isTurningAway;
    private Vector2 turnAwayDirection;
    private float turnAwayEndTime;
    private float lastTurnAwayTime = -999f;

    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void FixedUpdate()
    {
        if (target == null) return;

        if (isTurningAway)
        {
            MoveAwayFromEnemy();
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            Vector2 direction = (target.position - transform.position).normalized;

            UpdateFacing(direction.x);

            Vector2 newPosition = rb.position + direction * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            if (animator != null)
            {
                animator.SetFloat("Speed", moveSpeed);
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    private void MoveAwayFromEnemy()
    {
        if (Time.time >= turnAwayEndTime)
        {
            isTurningAway = false;
            return;
        }

        UpdateFacing(turnAwayDirection.x);

        Vector2 newPosition = rb.position + turnAwayDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);

        if (animator != null)
        {
            animator.SetFloat("Speed", moveSpeed);
        }
    }

    private void Update()
    {
        if (target == null) return;
        if (isTurningAway) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            TryAttack();
        }
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = target.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth == null)
        {
            playerHealth = target.GetComponentInChildren<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            int finalDamage = damage;

            if (DifficultyManager.Instance != null)
            {
                finalDamage = DifficultyManager.Instance.GetEnemyDamage(damage);
            }

            playerHealth.TakeDamage(finalDamage);
        }

        PlayerController playerController = target.GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = target.GetComponentInParent<PlayerController>();
        }

        if (playerController == null)
        {
            playerController = target.GetComponentInChildren<PlayerController>();
        }

        if (playerController != null)
        {
            Vector2 knockbackDirection = (target.position - transform.position).normalized;
            Vector2 force = knockbackDirection * knockbackForce;

            playerController.ApplyKnockback(force, knockbackDuration);
        }

        Debug.Log("Flying enemy attacked player");
    }

    private void StartTurnAway(GameObject otherEnemy)
    {
        if (Time.time < lastTurnAwayTime + turnAwayCooldown) return;

        lastTurnAwayTime = Time.time;

        Vector2 directionFromOtherEnemy = transform.position - otherEnemy.transform.position;

        if (directionFromOtherEnemy.sqrMagnitude < 0.01f)
        {
            directionFromOtherEnemy = facingRight ? Vector2.left : Vector2.right;
        }

        turnAwayDirection = directionFromOtherEnemy.normalized;
        turnAwayEndTime = Time.time + turnAwayDuration;
        isTurningAway = true;

        UpdateFacing(turnAwayDirection.x);

        Debug.Log("Flying enemy met another flying enemy and turned away");
    }

    private void UpdateFacing(float directionX)
    {
        if (Mathf.Abs(directionX) < 0.01f) return;

        if (directionX > 0f && !facingRight)
        {
            Flip();
        }
        else if (directionX < 0f && facingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private bool IsEnemyLayer(GameObject obj)
    {
        return (enemyLayer.value & (1 << obj.layer)) != 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsEnemyLayer(collision.gameObject))
        {
            StartTurnAway(collision.gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (IsEnemyLayer(collision.gameObject))
        {
            StartTurnAway(collision.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsEnemyLayer(collision.gameObject))
        {
            StartTurnAway(collision.gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (IsEnemyLayer(collision.gameObject))
        {
            StartTurnAway(collision.gameObject);
        }
    }
}