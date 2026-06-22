using System.Collections;
using UnityEngine;

public class FinalBossController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float stopDistance = 1.4f;

    [Header("Stage 2 Modifiers")]
    public float stage2MoveSpeedMultiplier = 1.35f;
    public float stage2DamageMultiplier = 1.5f;
    public float stage2CooldownMultiplier = 0.7f;

    [Header("Melee Attack")]
    public int meleeDamage = 2;
    public float meleeRange = 1.5f;
    public float meleeCooldown = 1.2f;
    public float meleeHitDelay = 0.2f;
    public float meleeKnockbackForce = 8f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public int projectileDamage = 1;
    public float projectileSpeed = 8f;
    public float projectileCooldown = 2f;

    [Header("Ground Slam")]
    public Transform slamPoint;
    public int slamDamage = 3;
    public float slamRadius = 3f;
    public float slamCooldown = 4f;
    public float slamWarningTime = 0.5f;

    [Header("Slam LineRenderer Visual")]
    public float slamLineDuration = 0.35f;
    public float slamLineWidth = 0.12f;
    public int slamCirclePoints = 64;
    public Color slamWarningColor = new Color(1f, 0.2f, 0f, 0.65f);
    public Color slamHitColor = new Color(1f, 0f, 0f, 1f);

    [Header("Stage 2 Dash Attack")]
    public float dashSpeed = 12f;
    public float dashTime = 0.25f;
    public float dashCooldown = 3f;
    public float dashTriggerDistance = 6f;
    public float dashHitRangeX = 1.2f;
    public float dashHitRangeY = 1.2f;
    public int dashDamage = 3;
    public float dashKnockbackForce = 10f;

    [Header("Stage 2 Fire LineRenderer")]
    public Transform stage2FirePoint;
    public float stage2FireRadius = 1.6f;
    public float stage2FireHeight = 1.2f;
    public int stage2FirePoints = 24;
    public float stage2FireLineWidth = 0.08f;
    public Color stage2FireColor = new Color(1f, 0.05f, 0f, 0.9f);
    public bool showStage2Fire = true;

    [Header("LineRenderer Sorting")]
    public string lineSortingLayerName = "Default";
    public int lineSortingOrder = 30;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;
    private int stage = 1;

    private bool isBusy = false;
    private bool isDead = false;

    private float lastMeleeTime = -999f;
    private float lastProjectileTime = -999f;
    private float lastSlamTime = -999f;
    private float lastDashTime = -999f;

    private bool facingRight = false;

    private LineRenderer stage2FireLine;
    private LineRenderer slamLine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        CreateLineRenderers();
        HideStage2Fire();
        HideSlamLine();
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
        if (stage == 2 && !isDead && showStage2Fire)
        {
            UpdateStage2Fire();
        }

        if (isDead) return;
        if (player == null) return;
        if (isBusy) return;

        FacePlayer();

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= meleeRange)
        {
            TryMeleeAttack();
            return;
        }

        if (stage == 2 && distanceToPlayer <= dashTriggerDistance)
        {
            TryDashAttack();
            return;
        }

        TrySlamAttack();
        TryProjectileAttack();
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (player == null)
        {
            SetSpeedAnimation(0f);
            return;
        }

        if (isBusy)
        {
            SetSpeedAnimation(0f);
            return;
        }

        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= stopDistance)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            SetSpeedAnimation(0f);
            return;
        }

        float direction = player.position.x > transform.position.x ? 1f : -1f;

        rb.velocity = new Vector2(direction * GetMoveSpeed(), rb.velocity.y);

        SetSpeedAnimation(Mathf.Abs(rb.velocity.x));
    }

    private void FacePlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x && !facingRight)
        {
            facingRight = true;

            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            facingRight = false;

            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void TryMeleeAttack()
    {
        if (Time.time < lastMeleeTime + GetCooldown(meleeCooldown)) return;

        lastMeleeTime = Time.time;
        StartCoroutine(MeleeAttackRoutine());
    }

    private IEnumerator MeleeAttackRoutine()
    {
        isBusy = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        SetSpeedAnimation(0f);

        SetAnimatorTrigger("Melee");

        yield return new WaitForSeconds(meleeHitDelay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, meleeRange);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = hit.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(GetFinalDamage(meleeDamage));
            }

            PlayerController playerController = hit.GetComponent<PlayerController>();

            if (playerController == null)
            {
                playerController = hit.GetComponentInParent<PlayerController>();
            }

            if (playerController != null)
            {
                Vector2 knockbackDirection =
                    ((Vector2)hit.transform.position - (Vector2)transform.position).normalized;

                if (knockbackDirection == Vector2.zero)
                {
                    knockbackDirection = facingRight ? Vector2.right : Vector2.left;
                }

                playerController.ApplyKnockback(knockbackDirection * meleeKnockbackForce, 0.15f);
            }

            Debug.Log("Final Boss hit player with melee");
            break;
        }

        yield return new WaitForSeconds(0.2f);

        isBusy = false;
    }

    private void TryProjectileAttack()
    {
        if (projectilePrefab == null) return;
        if (shootPoint == null) return;
        if (Time.time < lastProjectileTime + GetCooldown(projectileCooldown)) return;

        lastProjectileTime = Time.time;
        StartCoroutine(ProjectileAttackRoutine());
    }

    private IEnumerator ProjectileAttackRoutine()
    {
        isBusy = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        SetSpeedAnimation(0f);

        SetAnimatorTrigger("Shoot");

        yield return new WaitForSeconds(0.25f);

        if (player != null)
        {
            Vector2 direction = (player.position - shootPoint.position).normalized;

            GameObject projectileObject = Instantiate(
                projectilePrefab,
                shootPoint.position,
                Quaternion.identity
            );

            FinalBossProjectile projectile = projectileObject.GetComponent<FinalBossProjectile>();

            if (projectile == null)
            {
                projectile = projectileObject.GetComponentInChildren<FinalBossProjectile>();
            }

            if (projectile != null)
            {
                projectile.Init(direction, projectileSpeed, GetFinalDamage(projectileDamage));
            }
            else
            {
                Debug.LogWarning("FinalBossProjectile не найден на prefab снаряда. Повесь FinalBossProjectile на корневой объект Projectile Prefab.");
            }
        }

        yield return new WaitForSeconds(0.2f);

        isBusy = false;
    }

    private void TrySlamAttack()
    {
        if (slamPoint == null) return;
        if (Time.time < lastSlamTime + GetCooldown(slamCooldown)) return;

        lastSlamTime = Time.time;
        StartCoroutine(SlamAttackRoutine());
    }

    private IEnumerator SlamAttackRoutine()
    {
        isBusy = true;
        rb.velocity = new Vector2(0f, rb.velocity.y);
        SetSpeedAnimation(0f);

        SetAnimatorTrigger("Slam");

        Vector3 slamCenter = slamPoint != null ? slamPoint.position : transform.position;
        float finalSlamRadius = stage == 2 ? slamRadius * 1.25f : slamRadius;

        float warningTimer = 0f;

        while (warningTimer < slamWarningTime)
        {
            warningTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(warningTimer / slamWarningTime);
            float warningRadius = Mathf.Lerp(0.2f, finalSlamRadius, progress);

            DrawSlamCircle(slamCenter, warningRadius, slamWarningColor);

            yield return null;
        }

        HideSlamLine();

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(slamCenter, player.position);

            if (distanceToPlayer <= finalSlamRadius)
            {
                DealDamageToPlayer(slamDamage, slamCenter, 9f);
            }
        }

        float slamTimer = 0f;

        while (slamTimer < slamLineDuration)
        {
            slamTimer += Time.deltaTime;

            float progress = Mathf.Clamp01(slamTimer / slamLineDuration);
            float radius = Mathf.Lerp(0.2f, finalSlamRadius, progress);

            DrawSlamCircle(slamCenter, radius, slamHitColor);

            yield return null;
        }

        HideSlamLine();

        yield return new WaitForSeconds(0.15f);

        isBusy = false;
    }

    private void TryDashAttack()
    {
        if (Time.time < lastDashTime + GetCooldown(dashCooldown)) return;

        lastDashTime = Time.time;
        StartCoroutine(DashAttackRoutine());
    }

    private IEnumerator DashAttackRoutine()
    {
        isBusy = true;
        SetSpeedAnimation(0f);

        SetAnimatorTrigger("Dash");

        float direction = player.position.x > transform.position.x ? 1f : -1f;
        float timer = 0f;
        bool hitPlayer = false;

        while (timer < dashTime)
        {
            timer += Time.fixedDeltaTime;

            rb.velocity = new Vector2(direction * dashSpeed, rb.velocity.y);

            if (!hitPlayer && player != null)
            {
                float xDistance = Mathf.Abs(player.position.x - transform.position.x);
                float yDistance = Mathf.Abs(player.position.y - transform.position.y);

                if (xDistance <= dashHitRangeX && yDistance <= dashHitRangeY)
                {
                    hitPlayer = true;
                    DealDamageToPlayer(dashDamage, transform.position, dashKnockbackForce);
                }
            }

            yield return new WaitForFixedUpdate();
        }

        rb.velocity = new Vector2(0f, rb.velocity.y);
        SetSpeedAnimation(0f);

        yield return new WaitForSeconds(0.2f);

        isBusy = false;
    }

    private void DealDamageToPlayer(int baseDamage, Vector2 damageSource, float knockbackForce)
    {
        if (player == null) return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<PlayerHealth>();
        }

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(GetFinalDamage(baseDamage));
        }

        PlayerController playerController = player.GetComponent<PlayerController>();

        if (playerController == null)
        {
            playerController = player.GetComponentInParent<PlayerController>();
        }

        if (playerController != null)
        {
            Vector2 knockbackDirection = ((Vector2)player.position - damageSource).normalized;

            if (knockbackDirection == Vector2.zero)
            {
                knockbackDirection = facingRight ? Vector2.right : Vector2.left;
            }

            Vector2 force = knockbackDirection * knockbackForce;

            playerController.ApplyKnockback(force, 0.15f);
        }
    }

    private int GetFinalDamage(int baseDamage)
    {
        float damage = baseDamage;

        if (stage == 2)
        {
            damage *= stage2DamageMultiplier;
        }

        int finalDamage = Mathf.CeilToInt(damage);

        if (DifficultyManager.Instance != null)
        {
            finalDamage = DifficultyManager.Instance.GetMiniBossDamage(finalDamage);
        }

        return finalDamage;
    }

    private float GetMoveSpeed()
    {
        if (stage == 2)
        {
            return moveSpeed * stage2MoveSpeedMultiplier;
        }

        return moveSpeed;
    }

    private float GetCooldown(float baseCooldown)
    {
        if (stage == 2)
        {
            return baseCooldown * stage2CooldownMultiplier;
        }

        return baseCooldown;
    }

    public void EnterStage2()
    {
        if (stage == 2) return;

        stage = 2;

        SetAnimatorInteger("Stage", 2);

        // Если в Animator есть Trigger Stage2 — короткая анимация усиления проиграется.
        // Если такого параметра нет — ошибки не будет.
        SetAnimatorTrigger("Stage2");

        if (showStage2Fire)
        {
            UpdateStage2Fire();
        }

        Debug.Log("Final Boss entered Stage 2");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;
        isBusy = true;

        rb.velocity = Vector2.zero;
        SetSpeedAnimation(0f);

        HideStage2Fire();
        HideSlamLine();

        SetAnimatorTrigger("Die");

        Debug.Log("Final Boss died");
    }

    private void CreateLineRenderers()
    {
        GameObject fireObject = new GameObject("Stage2_Fire_Line");
        fireObject.transform.SetParent(transform);
        fireObject.transform.localPosition = Vector3.zero;

        stage2FireLine = fireObject.AddComponent<LineRenderer>();
        SetupLineRenderer(stage2FireLine, stage2FireLineWidth, stage2FireColor);
        stage2FireLine.loop = false;
        stage2FireLine.enabled = false;

        GameObject slamObject = new GameObject("Slam_Radius_Line");
        slamObject.transform.SetParent(null);
        slamObject.transform.position = Vector3.zero;

        slamLine = slamObject.AddComponent<LineRenderer>();
        SetupLineRenderer(slamLine, slamLineWidth, slamHitColor);
        slamLine.loop = true;
        slamLine.enabled = false;
    }

    private void SetupLineRenderer(LineRenderer line, float width, Color color)
    {
        if (line == null) return;

        line.useWorldSpace = true;
        line.startWidth = width;
        line.endWidth = width;
        line.positionCount = 0;

        Shader shader = Shader.Find("Sprites/Default");

        if (shader != null)
        {
            line.material = new Material(shader);
        }

        line.startColor = color;
        line.endColor = color;

        line.sortingLayerName = lineSortingLayerName;
        line.sortingOrder = lineSortingOrder;

        line.numCapVertices = 4;
        line.numCornerVertices = 4;
    }

    private void UpdateStage2Fire()
    {
        if (stage2FireLine == null) return;

        Transform point = stage2FirePoint != null ? stage2FirePoint : transform;
        Vector3 center = point.position;

        stage2FireLine.enabled = true;
        stage2FireLine.startWidth = stage2FireLineWidth;
        stage2FireLine.endWidth = stage2FireLineWidth;
        stage2FireLine.startColor = stage2FireColor;
        stage2FireLine.endColor = stage2FireColor;

        int pointCount = Mathf.Max(4, stage2FirePoints * 2);
        stage2FireLine.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);
            float x = Mathf.Lerp(-stage2FireRadius, stage2FireRadius, t);

            bool isFlameTip = i % 2 == 1;

            float y;

            if (isFlameTip)
            {
                y = Random.Range(0.35f, stage2FireHeight);
            }
            else
            {
                y = Random.Range(0f, 0.15f);
            }

            Vector3 position = center + new Vector3(x, y, 0f);
            stage2FireLine.SetPosition(i, position);
        }
    }

    private void HideStage2Fire()
    {
        if (stage2FireLine != null)
        {
            stage2FireLine.enabled = false;
        }
    }

    private void DrawSlamCircle(Vector3 center, float radius, Color color)
    {
        if (slamLine == null) return;

        slamLine.enabled = true;
        slamLine.startWidth = slamLineWidth;
        slamLine.endWidth = slamLineWidth;
        slamLine.startColor = color;
        slamLine.endColor = color;

        int pointCount = Mathf.Max(8, slamCirclePoints);
        slamLine.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float angle = (i / (float)pointCount) * Mathf.PI * 2f;

            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius * 0.35f;

            Vector3 position = center + new Vector3(x, y, 0f);
            slamLine.SetPosition(i, position);
        }
    }

    private void HideSlamLine()
    {
        if (slamLine != null)
        {
            slamLine.enabled = false;
        }
    }

    private void SetSpeedAnimation(float speed)
    {
        SetAnimatorFloat("Speed", speed);
    }

    private void SetAnimatorTrigger(string parameterName)
    {
        if (animator == null) return;
        if (!HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Trigger)) return;

        animator.SetTrigger(parameterName);
    }

    private void SetAnimatorFloat(string parameterName, float value)
    {
        if (animator == null) return;
        if (!HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Float)) return;

        animator.SetFloat(parameterName, value);
    }

    private void SetAnimatorInteger(string parameterName, int value)
    {
        if (animator == null) return;
        if (!HasAnimatorParameter(parameterName, AnimatorControllerParameterType.Int)) return;

        animator.SetInteger(parameterName, value);
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType parameterType)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == parameterName && parameter.type == parameterType)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        if (slamPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(slamPoint.position, slamRadius);
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(
            transform.position,
            new Vector3(dashHitRangeX * 2f, dashHitRangeY * 2f, 0f)
        );

        Transform firePoint = stage2FirePoint != null ? stage2FirePoint : transform;

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(firePoint.position, stage2FireRadius);
    }
}