using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Transform shootPoint;
    public GameObject projectilePrefab;

    [Header("Shooting")]
    public float detectionRange = 8f;
    public float fireCooldown = 1.5f;
    public float fireDelay = 0.25f;

    [Header("Turning")]
    public bool turnToPlayerWhenBehind = true;
    public float turnToPlayerRadius = 8f;

    [Header("Animation")]
    public Animator animator;

    private float lastFireTime = -999f;

    private EnemyHealth enemyHealth;
    private EnemyController enemyController;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyController = GetComponent<EnemyController>();

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
        if (enemyHealth != null && enemyHealth.IsDead()) return;
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (turnToPlayerWhenBehind && distanceToPlayer <= turnToPlayerRadius && !IsPlayerInFront())
        {
            TurnToPlayer();
            return;
        }

        if (distanceToPlayer <= detectionRange && IsPlayerInFront())
        {
            TryShootAtPlayer();
        }
    }

    private void TurnToPlayer()
    {
        if (enemyController != null)
        {
            enemyController.FaceTargetX(player.position.x);
        }
    }

    private bool IsPlayerInFront()
    {
        Vector2 directionToPlayer = player.position - transform.position;

        bool facingRight = IsFacingRight();

        if (facingRight && directionToPlayer.x >= 0f)
        {
            return true;
        }

        if (!facingRight && directionToPlayer.x <= 0f)
        {
            return true;
        }

        return false;
    }

    private bool IsFacingRight()
    {
        if (enemyController != null)
        {
            return enemyController.IsFacingRight();
        }

        return transform.localScale.x > 0f;
    }

    private void TryShootAtPlayer()
    {
        if (Time.time < lastFireTime + fireCooldown) return;

        lastFireTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        StartCoroutine(ShootWithDelay());
    }

    private IEnumerator ShootWithDelay()
    {
        yield return new WaitForSeconds(fireDelay);

        if (enemyHealth != null && enemyHealth.IsDead()) yield break;
        if (player == null) yield break;
        if (shootPoint == null) yield break;
        if (projectilePrefab == null) yield break;

        if (!IsPlayerInFront()) yield break;

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

        Debug.Log("Enemy shooter fired");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, turnToPlayerRadius);
    }
}