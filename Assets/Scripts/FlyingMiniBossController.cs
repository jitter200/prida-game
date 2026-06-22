using System.Collections;
using UnityEngine;

public class FlyingMiniBossController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    [Header("Beam Collision")]
    public bool stopBeamOnGround = false;

    [Header("Movement")]
    public float moveSpeed = 2.6f;
    public float horizontalOffset = 4f;
    public float verticalOffset = 2.5f;
    public float safeDistance = 2.5f;
    public bool stayOnRightSide = true;

    [Header("Arena Limits")]
    public bool useArenaLimits = true;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = 0f;
    public float maxY = 10f;

    [Header("Melee Attack")]
    public int meleeDamage = 14;
    public float meleeRange = 1.4f;
    public float meleeCooldown = 1.4f;
    public float meleeKnockbackForce = 8f;
    public float meleeKnockbackDuration = 0.15f;

    [Header("Ranged Attack")]
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public int projectileDamage = 9;
    public float projectileSpeed = 8f;
    public float rangedCooldown = 2f;

    [Header("Down Beam Attack")]
    public Transform beamPoint;
    public int beamDamage = 20;
    public float beamLength = 8f;
    public float beamWidth = 1.2f;
    public float beamWarningTime = 0.8f;
    public float beamActiveTime = 0.35f;
    public float beamCooldown = 6.5f;
    public LayerMask playerLayer;
    public LayerMask groundLayer;

    [Header("Beam Charge Visual")]
    public int chargeRingCount = 3;
    public int chargeCirclePoints = 48;
    public float chargeStartRadius = 0.2f;
    public float chargeEndRadius = 1.0f;
    public float chargeLineWidth = 0.04f;

    [Header("Beam Warning Visual")]
    public float warningLineWidth = 0.08f;

    [Header("Beam Visual")]
    public int beamSegments = 10;
    public float beamStartWidth = 0.75f;
    public float beamEndWidth = 0.45f;
    public float beamJitter = 0.08f;

    [Header("Beam Colors")]
    public Color chargeColor = new Color(0.7f, 0.2f, 1f, 0.9f);
    public Color warningColor = new Color(1f, 0.1f, 0.1f, 0.7f);
    public Color beamColorStart = new Color(1f, 0.2f, 1f, 1f);
    public Color beamColorEnd = new Color(0.4f, 0.1f, 1f, 0.9f);

    [Header("Sorting")]
    public string sortingLayerName = "Default";
    public int sortingOrder = 20;

    [Header("Animation")]
    public Animator animator;

    private Rigidbody2D rb;

    private float lastMeleeTime = -999f;
    private float lastRangedTime = -999f;
    private float lastBeamTime = -999f;

    private bool isUsingBeam = false;
    private bool facingRight = true;

    private LineRenderer[] chargeRings;
    private LineRenderer warningLine;
    private LineRenderer beamLine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (beamPoint == null)
        {
            beamPoint = transform;
        }

        CreateBeamVisuals();
        HideBeamVisuals();
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

    private void FixedUpdate()
    {
        if (player == null) return;

        if (isUsingBeam)
        {
            SetSpeedAnimation(0f);
            return;
        }

        MoveAroundPlayer();
    }

    private void Update()
    {
        if (player == null) return;
        if (isUsingBeam) return;

        FacePlayer();

        float distance = Vector2.Distance(transform.position, player.position);

        if (TryBeamAttack())
        {
            return;
        }

        if (distance <= meleeRange)
        {
            TryMeleeAttack();
        }
        else
        {
            TryRangedAttack();
        }
    }

    private void MoveAroundPlayer()
    {
        float side = stayOnRightSide ? 1f : -1f;

        Vector2 targetPosition = new Vector2(
            player.position.x + horizontalOffset * side,
            player.position.y + verticalOffset
        );

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer < safeDistance)
        {
            Vector2 awayFromPlayer = ((Vector2)transform.position - (Vector2)player.position).normalized;

            if (awayFromPlayer == Vector2.zero)
            {
                awayFromPlayer = stayOnRightSide ? Vector2.right : Vector2.left;
            }

            targetPosition = (Vector2)player.position + awayFromPlayer * safeDistance;
            targetPosition.y += verticalOffset;
        }

        if (useArenaLimits)
        {
            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        Vector2 oldPosition = rb.position;

        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            targetPosition,
            moveSpeed * Time.fixedDeltaTime
        );

        if (useArenaLimits)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        rb.MovePosition(newPosition);

        float speed = Vector2.Distance(oldPosition, newPosition) / Time.fixedDeltaTime;
        SetSpeedAnimation(speed);
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

    private void SetSpeedAnimation(float speed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
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

        if (playerHealth == null)
        {
            playerHealth = player.GetComponentInParent<PlayerHealth>();
        }

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

        if (playerController == null)
        {
            playerController = player.GetComponentInParent<PlayerController>();
        }

        if (playerController != null)
        {
            Vector2 knockbackDirection = (player.position - transform.position).normalized;
            Vector2 force = knockbackDirection * meleeKnockbackForce;

            playerController.ApplyKnockback(force, meleeKnockbackDuration);
        }

        Debug.Log("Flying mini-boss melee attack");
    }

    private void TryRangedAttack()
    {
        if (Time.time < lastRangedTime + rangedCooldown) return;
        if (projectilePrefab == null || shootPoint == null) return;

        lastRangedTime = Time.time;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Vector2 direction = (player.position - shootPoint.position).normalized;

        GameObject projectileObject = Instantiate(
            projectilePrefab,
            shootPoint.position,
            Quaternion.identity
        );

        FlyingMiniBossProjectile projectile = projectileObject.GetComponent<FlyingMiniBossProjectile>();

        if (projectile != null)
        {
            int finalDamage = projectileDamage;

            if (DifficultyManager.Instance != null)
            {
                finalDamage = DifficultyManager.Instance.GetMiniBossDamage(projectileDamage);
            }

            projectile.Init(direction, projectileSpeed, finalDamage);
        }

        Debug.Log("Flying mini-boss ranged attack");
    }

    private bool TryBeamAttack()
    {
        if (isUsingBeam) return false;
        if (Time.time < lastBeamTime + beamCooldown) return false;
        if (beamPoint == null) return false;

        lastBeamTime = Time.time;

        StartCoroutine(BeamAttackRoutine());

        return true;
    }

    private IEnumerator BeamAttackRoutine()
    {
        isUsingBeam = true;

        rb.velocity = Vector2.zero;
        SetSpeedAnimation(0f);

        //if (animator != null)
       // {
           // animator.SetTrigger("BigBeam");
        //}

        yield return StartCoroutine(BeamChargeRoutine());
        yield return StartCoroutine(BeamFireRoutine());

        HideBeamVisuals();

        isUsingBeam = false;
    }

    private IEnumerator BeamChargeRoutine()
    {
        SetChargeVisible(true);
        SetWarningVisible(true);
        SetBeamVisible(false);

        float timer = 0f;

        while (timer < beamWarningTime)
        {
            timer += Time.deltaTime;

            float progress = Mathf.Clamp01(timer / beamWarningTime);

            UpdateChargeRings(progress);
            UpdateWarningLine(progress);

            yield return null;
        }
    }

    private IEnumerator BeamFireRoutine()
    {
        SetChargeVisible(false);
        SetWarningVisible(false);
        SetBeamVisible(true);

        DealBeamDamage();

        float timer = 0f;

        while (timer < beamActiveTime)
        {
            timer += Time.deltaTime;

            UpdateBeamLine();

            yield return null;
        }

        SetBeamVisible(false);
    }

    private void DealBeamDamage()
    {
        Vector2 start = beamPoint.position;
        Vector2 end = GetBeamEndPosition();

        float length = Vector2.Distance(start, end);
        Vector2 center = (start + end) * 0.5f;

        Collider2D[] hits = Physics2D.OverlapBoxAll(
            center,
            new Vector2(beamWidth, length),
            0f,
            playerLayer
        );

        foreach (Collider2D hit in hits)
        {
            PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = hit.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth != null)
            {
                int finalDamage = beamDamage;

                if (DifficultyManager.Instance != null)
                {
                    finalDamage = DifficultyManager.Instance.GetMiniBossDamage(beamDamage);
                }

                playerHealth.TakeDamage(finalDamage);
            }

            PlayerController playerController = hit.GetComponent<PlayerController>();

            if (playerController == null)
            {
                playerController = hit.GetComponentInParent<PlayerController>();
            }

            if (playerController != null)
            {
                Vector2 knockbackDirection = ((Vector2)hit.transform.position - start).normalized;

                if (knockbackDirection.sqrMagnitude < 0.01f)
                {
                    knockbackDirection = Vector2.down;
                }

                playerController.ApplyKnockback(knockbackDirection * meleeKnockbackForce, meleeKnockbackDuration);
            }
        }

        Debug.Log("Flying mini-boss beam attack");
    }

    private Vector2 GetBeamEndPosition()
    {
        Vector2 start = beamPoint.position;

        if (!stopBeamOnGround)
        {
            return start + Vector2.down * beamLength;
        }

        RaycastHit2D hit = Physics2D.Raycast(
            start,
            Vector2.down,
            beamLength,
            groundLayer
        );

        if (hit.collider != null)
        {
            return hit.point;
        }

        return start + Vector2.down * beamLength;
    }

    private void CreateBeamVisuals()
    {
        chargeRings = new LineRenderer[chargeRingCount];

        for (int i = 0; i < chargeRingCount; i++)
        {
            GameObject ringObject = new GameObject("BeamChargeRing_" + i);
            ringObject.transform.SetParent(transform);
            ringObject.transform.localPosition = Vector3.zero;

            LineRenderer line = ringObject.AddComponent<LineRenderer>();
            SetupLineRenderer(line);

            line.loop = true;
            line.positionCount = chargeCirclePoints;
            line.startWidth = chargeLineWidth;
            line.endWidth = chargeLineWidth;
            line.startColor = chargeColor;
            line.endColor = chargeColor;

            chargeRings[i] = line;
        }

        GameObject warningObject = new GameObject("BeamWarningLine");
        warningObject.transform.SetParent(transform);
        warningObject.transform.localPosition = Vector3.zero;

        warningLine = warningObject.AddComponent<LineRenderer>();
        SetupLineRenderer(warningLine);
        warningLine.positionCount = 2;
        warningLine.startWidth = warningLineWidth;
        warningLine.endWidth = warningLineWidth;
        warningLine.startColor = warningColor;
        warningLine.endColor = warningColor;

        GameObject beamObject = new GameObject("BeamLine");
        beamObject.transform.SetParent(transform);
        beamObject.transform.localPosition = Vector3.zero;

        beamLine = beamObject.AddComponent<LineRenderer>();
        SetupLineRenderer(beamLine);
        beamLine.positionCount = beamSegments;
        beamLine.startWidth = beamStartWidth;
        beamLine.endWidth = beamEndWidth;
        beamLine.startColor = beamColorStart;
        beamLine.endColor = beamColorEnd;
    }

    private void SetupLineRenderer(LineRenderer line)
    {
        line.useWorldSpace = true;
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.sortingLayerName = sortingLayerName;
        line.sortingOrder = sortingOrder;
        line.numCapVertices = 6;
        line.numCornerVertices = 6;
    }

    private void UpdateChargeRings(float progress)
    {
        Vector3 center = beamPoint.position;

        for (int i = 0; i < chargeRings.Length; i++)
        {
            float offset = i / (float)chargeRings.Length;
            float ringProgress = Mathf.Repeat(progress + offset, 1f);

            float radius = Mathf.Lerp(chargeStartRadius, chargeEndRadius, ringProgress);
            float alpha = Mathf.Lerp(0.9f, 0.15f, ringProgress);

            Color color = chargeColor;
            color.a = alpha;

            chargeRings[i].startColor = color;
            chargeRings[i].endColor = color;

            for (int p = 0; p < chargeCirclePoints; p++)
            {
                float angle = (p / (float)chargeCirclePoints) * Mathf.PI * 2f;

                Vector3 point = center + new Vector3(
                    Mathf.Cos(angle) * radius,
                    Mathf.Sin(angle) * radius,
                    0f
                );

                chargeRings[i].SetPosition(p, point);
            }
        }
    }

    private void UpdateWarningLine(float progress)
    {
        Vector2 start = beamPoint.position;
        Vector2 end = GetBeamEndPosition();

        Color color = warningColor;
        color.a = Mathf.Lerp(0.25f, 0.85f, progress);

        warningLine.startColor = color;
        warningLine.endColor = color;

        warningLine.startWidth = Mathf.Lerp(0.04f, warningLineWidth, progress);
        warningLine.endWidth = Mathf.Lerp(0.04f, warningLineWidth, progress);

        warningLine.SetPosition(0, start);
        warningLine.SetPosition(1, end);
    }

    private void UpdateBeamLine()
    {
        Vector2 start = beamPoint.position;
        Vector2 end = GetBeamEndPosition();

        for (int i = 0; i < beamSegments; i++)
        {
            float t = i / (float)(beamSegments - 1);

            Vector2 point = Vector2.Lerp(start, end, t);

            if (i != 0 && i != beamSegments - 1)
            {
                point.x += Random.Range(-beamJitter, beamJitter);
            }

            beamLine.SetPosition(i, point);
        }
    }

    private void HideBeamVisuals()
    {
        SetChargeVisible(false);
        SetWarningVisible(false);
        SetBeamVisible(false);
    }

    private void SetChargeVisible(bool value)
    {
        if (chargeRings == null) return;

        foreach (LineRenderer ring in chargeRings)
        {
            if (ring != null)
            {
                ring.enabled = value;
            }
        }
    }

    private void SetWarningVisible(bool value)
    {
        if (warningLine != null)
        {
            warningLine.enabled = value;
        }
    }

    private void SetBeamVisible(bool value)
    {
        if (beamLine != null)
        {
            beamLine.enabled = value;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (beamPoint != null)
        {
            Vector3 start = beamPoint.position;
            Vector3 end = start + Vector3.down * beamLength;

            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(start, end);

            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                (start + end) * 0.5f,
                new Vector3(beamWidth, beamLength, 0.1f)
            );
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}