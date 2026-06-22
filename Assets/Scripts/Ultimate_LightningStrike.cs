using UnityEngine;

public class Ultimate_LightningStrike : MonoBehaviour
{
    [Header("Lightning Settings")]
    public int damage = 8;
    public float radius = 1.5f;
    public float maxCastRange = 8f;
    public float cooldown = 12f;
    public LayerMask enemyLayer;

    [Header("FX")]
    public GameObject lightningFxPrefab;

    private float lastUseTime = -999f;

    public void Use()
    {
        if (!CanUse()) return;

        Vector3 strikePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        strikePosition.z = 0f;

        Vector2 directionToMouse = strikePosition - transform.position;

        if (directionToMouse.magnitude > maxCastRange)
        {
            directionToMouse = directionToMouse.normalized * maxCastRange;
            strikePosition = transform.position + (Vector3)directionToMouse;
        }

        SpawnLightningFX(strikePosition);

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            strikePosition,
            radius,
            enemyLayer
        );

        int finalDamage = damage;

        if (PlayerStats.Instance != null)
        {
            finalDamage = PlayerStats.Instance.GetTotalDamage(damage);
        }

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(finalDamage);
            }

            MiniBossHealth miniBossHealth = enemy.GetComponent<MiniBossHealth>();

            if (miniBossHealth != null)
            {
                miniBossHealth.TakeDamage(finalDamage);
            }
        }

        lastUseTime = Time.time;

        Debug.Log("Lightning Strike used. Hit enemies: " + enemies.Length);
    }

    private void SpawnLightningFX(Vector3 strikePosition)
    {
        if (lightningFxPrefab == null) return;

        GameObject fxObject = Instantiate(
            lightningFxPrefab,
            strikePosition,
            Quaternion.identity
        );

        LightningStrikeFX fx = fxObject.GetComponent<LightningStrikeFX>();

        if (fx != null)
        {
            fx.Play(strikePosition, radius);
        }
        else
        {
            Destroy(fxObject, 1f);
        }
    }

    private bool CanUse()
    {
        if (Time.time < lastUseTime + cooldown)
        {
            Debug.Log("Lightning Strike cooldown");
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxCastRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, lastUseTime + cooldown - Time.time);
    }

    public float GetCooldownDuration()
    {
        return cooldown;
    }
}