using UnityEngine;

public class MiniBossController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    [Header("Detection")]
    public float detectionRange = 10f;
    public float meleeRange = 1.4f;

    [Header("Movement")]
    public float moveSpeed = 2.5f;

    [Header("Melee Attack")]
    public int meleeDamage = 2;
    public float meleeCooldown = 1.2f;
    public float knockbackForceX = 10f;
    public float knockbackForceY = 4f;
    public float knockbackDuration = 0.2f;

    [Header("Shooting")]
    public float shootCooldown = 2f;

    [Header("Dash")]
    public float dashSpeed = 12f;
    public float dashTime = 0.2f;
    public float dashCooldown = 4f;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private MiniBossHealth miniBossHealth;

    private float lastMeleeTime = -999f;
    private float lastShootTime = -999f;
    private float lastDashTime = -999f;

    private bool isDashing;
    private bool facingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        miniBossHealth = GetComponent<MiniBossHealth>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (miniBossHealth != null && miniBossHealth.IsDead()) return;
        if (player == null) return;

        if (miniBossHealth != null && miniBossHealth.IsParrying())
        {
            StopMoving();
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
        {
            StopMoving();
            return;
        }

        FacePlayer();

        if (distanceToPlayer <= meleeRange)
        {
            StopMoving();
            TryMeleeAttack();
            return;
        }

        if (CanDash(distanceToPlayer))
        {
            StartCoroutine(DashToPlayer());
            return;
        }

        MoveToPlayer();
        TryShoot();
    }

    private void MoveToPlayer()
    {
        if (isDashing) return;

        float direction = player.position.x > transform.position.x ? 1f : -1f;

        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    private void StopMoving()
    {
        if (isDashing) return;

        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
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

    private void TryMeleeAttack()
    {
        if (Time.time < lastMeleeTime + meleeCooldown) return;

        lastMeleeTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Melee");
        }

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            int finalDamage = meleeDamage;

            if (DifficultyManager.Instance != null)
            {
                finalDamage = DifficultyManager.Instance.GetMiniBossDamage(meleeDamage);
            }

            playerHealth.TakeDamage(finalDamage);
        }

        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            Vector2 direction = player.position - transform.position;
            direction.Normalize();

            Vector2 knockback = new Vector2(
                direction.x * knockbackForceX,
                knockbackForceY
            );

            playerController.ApplyKnockback(knockback, knockbackDuration);
        }

        Debug.Log("MiniBoss melee attack");
    }

    private void TryShoot()
    {
        if (projectilePrefab == null || shootPoint == null) return;
        if (Time.time < lastShootTime + shootCooldown) return;

        lastShootTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Vector2 direction = player.position - shootPoint.position;

        GameObject projectile = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        EnemyProjectile enemyProjectile = projectile.GetComponent<EnemyProjectile>();

        if (enemyProjectile != null)
        {
            enemyProjectile.Init(direction);
        }

        Debug.Log("MiniBoss shoot");
    }

    private bool CanDash(float distanceToPlayer)
    {
        if (isDashing) return false;
        if (Time.time < lastDashTime + dashCooldown) return false;

        return distanceToPlayer > meleeRange && distanceToPlayer < detectionRange;
    }

    private System.Collections.IEnumerator DashToPlayer()
    {
        isDashing = true;
        lastDashTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }

        float direction = player.position.x > transform.position.x ? 1f : -1f;

        rb.velocity = new Vector2(direction * dashSpeed, 0f);

        Debug.Log("MiniBoss dash");

        yield return new WaitForSeconds(dashTime);

        isDashing = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}