using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 7f;
    public float jumpForce = 14f;

    [Header("Animation")]
    public Animator animator;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Breakables")]
    public LayerMask breakableLayer;

    [Header("Jump")]
    public int maxJumps = 2;
    private int jumpsLeft;

    [Header("Dash Audio")]
    public AudioSource audioSource;
    public AudioClip dashSound;

    [Range(0f, 1f)]
    public float dashSoundVolume = 0.8f;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashTime = 0.15f;
    public float dashCooldown = 0.5f;

    [Header("Knockback")]
    private bool isKnockedBack;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 1;
    public LayerMask enemyLayer;

    [Header("Ledge Grab")]
    public Transform wallCheck;
    public Transform ledgeCheck;
    public float ledgeCheckRadius = 0.15f;
    public Vector2 ledgeHangOffset = new Vector2(0.35f, -0.55f);
    public float ledgeClimbDelay = 0.4f;
    private bool isClimbingLedge;

    [Header("Bow")]
    public GameObject arrowPrefab;
    public Transform bowShootPoint;

    [Header("Parry")]
    public KeyCode parryKey = KeyCode.Z;
    public float parryDuration = 0.25f;
    public float parryCooldown = 0.6f;

    public LayerMask puzzleTorchLayer;

    [Header("Weapons")]
    public WeaponData sword = new WeaponData
    {
        weaponName = "Sword",
        damage = 1,
        attackRange = 0.5f,
        attackCooldown = 0.4f
    };

    public WeaponData heavySword = new WeaponData
    {
        weaponName = "HeavySword",
        damage = 3,
        attackRange = 0.7f,
        attackCooldown = 0.8f
    };

    public WeaponData bow = new WeaponData
    {
        weaponName = "Bow",
        damage = 2,
        attackRange = 6f,
        attackCooldown = 0.7f
    };

    public WeaponData spear = new WeaponData
    {
        weaponName = "Spear",
        damage = 2,
        attackRange = 1.2f,
        attackCooldown = 0.55f
    };

    private Rigidbody2D rb;

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private bool isParrying = false;
    private bool canParry = true;

    private bool isTouchingWall;
    private bool isTouchingLedge;
    private bool isHanging;
    private Vector2 ledgePosition;
    private float originalGravity;

    private bool isDashing;
    private bool canDash = true;
    private float dashDirection;

    private WeaponData currentWeapon;
    private bool canAttack = true;

    private bool isLastAttackFromMobile;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
        currentWeapon = sword;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (PlayerLoadout.Instance != null)
        {
            EquipWeapon(PlayerLoadout.Instance.currentWeapon);
        }
        else
        {
            currentWeapon = sword;
        }
    }

    private void Update()
    {
        bool pointerOverUI = IsPointerOverUI();

        moveInput = Input.GetAxisRaw("Horizontal");

        if (MobileInput.Instance != null && Mathf.Abs(MobileInput.Instance.horizontal) > 0.01f)
        {
            moveInput = MobileInput.Instance.horizontal;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }

        bool mobileInteractInput = MobileInput.Instance != null && MobileInput.Instance.interactPressed;

        if (isHanging)
        {
            if ((Input.GetKeyDown(KeyCode.X) || mobileInteractInput) && !isClimbingLedge)
            {
                StartCoroutine(ClimbLedgeWithDelay());
            }

            if (Input.GetKeyDown(KeyCode.S) && !isClimbingLedge)
            {
                DropFromLedge();
            }

            return;
        }

        bool parryInput =
            Input.GetKeyDown(parryKey) ||
            (MobileInput.Instance != null && MobileInput.Instance.parryPressed);

        if (parryInput && canParry)
        {
            StartCoroutine(Parry());
        }

        bool mobileAttackInput =
            MobileInput.Instance != null &&
            MobileInput.Instance.attackPressed;

        bool mouseAttackInput =
            !pointerOverUI &&
            Input.GetMouseButtonDown(0);

        bool keyboardAttackInput =
            Input.GetKeyDown(KeyCode.J);

        bool attackInput =
            mobileAttackInput ||
            mouseAttackInput ||
            keyboardAttackInput;

        if (attackInput && canAttack && !isDashing && !isHanging && !isParrying)
        {
            isLastAttackFromMobile = mobileAttackInput;
            StartCoroutine(Attack());
        }

        bool jumpInput =
            Input.GetKeyDown(KeyCode.Space) ||
            (MobileInput.Instance != null && MobileInput.Instance.jumpPressed);

        if (jumpInput && jumpsLeft > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpsLeft--;
        }

        bool dashInput =
            Input.GetKeyDown(KeyCode.LeftShift) ||
            (MobileInput.Instance != null && MobileInput.Instance.dashPressed);

        if (dashInput && canDash)
        {
            StartCoroutine(Dash());
        }

        CheckLedgeGrab();

        UpdateAnimator();
        Flip();
    }

    private void FixedUpdate()
    {
        if (isDashing || isHanging || isKnockedBack) return;

        float finalMoveSpeed = moveSpeed;

        if (PlayerStats.Instance != null)
        {
            finalMoveSpeed = PlayerStats.Instance.GetTotalSpeed(moveSpeed);
        }

        rb.velocity = new Vector2(moveInput * finalMoveSpeed, rb.velocity.y);
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null) return false;

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                {
                    return true;
                }
            }
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    private void Flip()
    {
        if (moveInput > 0 && !facingRight)
        {
            facingRight = true;
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (moveInput < 0 && facingRight)
        {
            facingRight = false;
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (attackPoint == null) return;

        Gizmos.color = Color.red;

        float range = attackRange;

        if (currentWeapon != null)
        {
            range = currentWeapon.attackRange;
        }

        Gizmos.DrawWireSphere(attackPoint.position, range);

        if (wallCheck != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(wallCheck.position, ledgeCheckRadius);
        }

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(ledgeCheck.position, ledgeCheckRadius);
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;

        PlayDashSound();

        if (animator != null)
        {
            animator.SetBool("IsDashing", true);
        }

        dashDirection = facingRight ? 1f : -1f;

        float savedGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.velocity = new Vector2(dashDirection * dashSpeed, 0f);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = savedGravity;
        isDashing = false;

        if (animator != null)
        {
            animator.SetBool("IsDashing", false);
        }

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private System.Collections.IEnumerator Attack()
    {
        canAttack = false;

        if (animator != null)
        {
            animator.SetBool("IsAttacking", true);
        }

        int finalDamage = currentWeapon.damage;

        if (PlayerStats.Instance != null)
        {
            finalDamage = PlayerStats.Instance.GetTotalDamage(currentWeapon.damage);
        }

        if (currentWeapon.weaponName == "Bow")
        {
            ShootBow(finalDamage);
        }
        else
        {
            MeleeAttack(finalDamage);
        }

        Debug.Log("Attack with " + currentWeapon.weaponName + " Damage: " + finalDamage);

        yield return new WaitForSeconds(0.25f);

        if (animator != null)
        {
            animator.SetBool("IsAttacking", false);
        }

        yield return new WaitForSeconds(currentWeapon.attackCooldown);

        isLastAttackFromMobile = false;
        canAttack = true;
    }

    private void CheckLedgeGrab()
    {
        if (isGrounded || isHanging) return;

        isTouchingWall = Physics2D.OverlapCircle(
            wallCheck.position,
            ledgeCheckRadius,
            groundLayer
        );

        isTouchingLedge = Physics2D.OverlapCircle(
            ledgeCheck.position,
            ledgeCheckRadius,
            groundLayer
        );

        bool pressingToWall =
            (facingRight && moveInput > 0) ||
            (!facingRight && moveInput < 0);

        if (isTouchingWall && !isTouchingLedge && pressingToWall && rb.velocity.y <= 0)
        {
            StartLedgeHang();
        }
    }

    private void StartLedgeHang()
    {
        isHanging = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        float direction = facingRight ? 1f : -1f;

        ledgePosition = new Vector2(
            wallCheck.position.x - ledgeHangOffset.x * direction,
            wallCheck.position.y + ledgeHangOffset.y
        );

        transform.position = ledgePosition;

        if (animator != null)
        {
            animator.SetBool("IsHanging", true);
            animator.SetFloat("Speed", 0f);
        }
    }

    private System.Collections.IEnumerator ClimbLedgeWithDelay()
    {
        isClimbingLedge = true;

        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        yield return new WaitForSeconds(ledgeClimbDelay);

        ClimbLedge();

        isClimbingLedge = false;
    }

    private void ClimbLedge()
    {
        isHanging = false;
        rb.gravityScale = originalGravity;

        if (animator != null)
        {
            animator.SetBool("IsHanging", false);
        }

        float direction = facingRight ? 1f : -1f;

        transform.position = new Vector2(
            transform.position.x + 0.8f * direction,
            transform.position.y + 1.2f
        );
    }

    private void DropFromLedge()
    {
        isHanging = false;
        rb.gravityScale = originalGravity;

        if (animator != null)
        {
            animator.SetBool("IsHanging", false);
        }
    }

    private System.Collections.IEnumerator Parry()
    {
        canParry = false;
        isParrying = true;

        if (animator != null)
        {
            animator.SetBool("IsParrying", true);
        }

        Debug.Log("Player parry");

        yield return new WaitForSeconds(parryDuration);

        isParrying = false;

        if (animator != null)
        {
            animator.SetBool("IsParrying", false);
        }

        yield return new WaitForSeconds(parryCooldown);

        canParry = true;
    }

    public void EquipWeapon(string weaponName)
    {
        if (weaponName == "Sword")
        {
            currentWeapon = sword;

            if (animator != null)
            {
                animator.SetInteger("Weapon", 0);
            }
        }
        else if (weaponName == "HeavySword")
        {
            currentWeapon = heavySword;

            if (animator != null)
            {
                animator.SetInteger("Weapon", 1);
            }
        }
        else if (weaponName == "Bow")
        {
            currentWeapon = bow;

            if (animator != null)
            {
                animator.SetInteger("Weapon", 2);
            }
        }
        else if (weaponName == "Spear")
        {
            currentWeapon = spear;

            if (animator != null)
            {
                animator.SetInteger("Weapon", 3);
            }
        }
        else
        {
            Debug.LogWarning("Unknown weapon: " + weaponName);

            currentWeapon = sword;
            weaponName = "Sword";

            if (animator != null)
            {
                animator.SetInteger("Weapon", 0);
            }
        }

        if (PlayerLoadout.Instance != null)
        {
            PlayerLoadout.Instance.EquipWeapon(weaponName);
        }

        Debug.Log("Current weapon: " + currentWeapon.weaponName);
    }

    public void ApplyKnockback(Vector2 force, float duration)
    {
        if (isKnockedBack) return;

        StartCoroutine(KnockbackRoutine(force, duration));
    }

    private System.Collections.IEnumerator KnockbackRoutine(Vector2 force, float duration)
    {
        isKnockedBack = true;

        rb.velocity = Vector2.zero;
        rb.velocity = force;

        yield return new WaitForSeconds(duration);

        isKnockedBack = false;
    }

    private void MeleeAttack(int finalDamage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
            attackPoint.position,
            currentWeapon.attackRange,
            enemyLayer
        );

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth == null)
            {
                enemyHealth = enemy.GetComponentInParent<EnemyHealth>();
            }

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(finalDamage);
            }

            MiniBossHealth miniBossHealth = enemy.GetComponent<MiniBossHealth>();

            if (miniBossHealth == null)
            {
                miniBossHealth = enemy.GetComponentInParent<MiniBossHealth>();
            }

            if (miniBossHealth != null)
            {
                miniBossHealth.TakeDamage(finalDamage);
            }

            FinalBossHealth finalBossHealth = enemy.GetComponent<FinalBossHealth>();

            if (finalBossHealth == null)
            {
                finalBossHealth = enemy.GetComponentInParent<FinalBossHealth>();
            }

            if (finalBossHealth != null)
            {
                finalBossHealth.TakeDamage(finalDamage);
            }
        }

        Collider2D[] hitTorches = Physics2D.OverlapCircleAll(
            attackPoint.position,
            currentWeapon.attackRange,
            puzzleTorchLayer
        );

        foreach (Collider2D torchCollider in hitTorches)
        {
            PuzzleTorch torch = torchCollider.GetComponent<PuzzleTorch>();

            if (torch == null)
            {
                torch = torchCollider.GetComponentInParent<PuzzleTorch>();
            }

            if (torch != null)
            {
                torch.HitByPlayer();
            }
        }

        Collider2D[] hitBreakables = Physics2D.OverlapCircleAll(
            attackPoint.position,
            currentWeapon.attackRange,
            breakableLayer
        );

        foreach (Collider2D breakableCollider in hitBreakables)
        {
            BreakableCage cage = breakableCollider.GetComponent<BreakableCage>();

            if (cage == null)
            {
                cage = breakableCollider.GetComponentInParent<BreakableCage>();
            }

            if (cage != null)
            {
                cage.Break();
            }
        }
    }

    private void UpdateAnimator()
    {
        if (animator == null) return;

        if (isHanging)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        animator.SetFloat("Speed", Mathf.Abs(moveInput));
    }
    private void PlayDashSound()
    {
        if (audioSource == null) return;
        if (dashSound == null) return;

        audioSource.PlayOneShot(dashSound, dashSoundVolume);
    }
    private void ShootBow(int finalDamage)
    {
        if (arrowPrefab == null || bowShootPoint == null)
        {
            Debug.LogWarning("Arrow prefab or BowShootPoint is not assigned");
            return;
        }

        Vector2 rawDirection;

        if (isLastAttackFromMobile || IsPointerOverUI())
        {
            rawDirection = facingRight ? Vector2.right : Vector2.left;
        }
        else
        {
            if (Camera.main == null)
            {
                rawDirection = facingRight ? Vector2.right : Vector2.left;
            }
            else
            {
                Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPosition.z = 0f;
                rawDirection = mouseWorldPosition - bowShootPoint.position;
            }
        }

        if (rawDirection.sqrMagnitude < 0.01f)
        {
            rawDirection = facingRight ? Vector2.right : Vector2.left;
        }

        Vector2 shootDirection = rawDirection.normalized;

        if (facingRight)
        {
            if (shootDirection.x < 0f)
            {
                shootDirection.x = 0f;
                shootDirection = shootDirection.normalized;
            }
        }
        else
        {
            if (shootDirection.x > 0f)
            {
                shootDirection.x = 0f;
                shootDirection = shootDirection.normalized;
            }
        }

        if (shootDirection == Vector2.zero)
        {
            shootDirection = facingRight ? Vector2.right : Vector2.left;
        }

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        GameObject arrow = Instantiate(
            arrowPrefab,
            bowShootPoint.position,
            Quaternion.Euler(0f, 0f, angle)
        );

        PlayerArrow playerArrow = arrow.GetComponent<PlayerArrow>();

        if (playerArrow != null)
        {
            playerArrow.Init(shootDirection, finalDamage);
        }
    }
}