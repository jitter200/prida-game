using UnityEngine;

public class Ultimate_Shockwave : MonoBehaviour
{
    [Header("Shockwave Settings")]
    public float radius = 3f;
    public int damage = 5;
    public float cooldown = 8f;
    public LayerMask enemyLayer;
    [Header("FX")]
    public GameObject shockwaveFxPrefab;

    private float lastUseTime = -999f;

    public void Use()
    {
        if (!CanUse()) return;

        Collider2D[] enemies = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            enemyLayer
        );

        foreach (Collider2D enemy in enemies)
        {
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(GetFinalDamage());
            }
        }
        SpawnShockwaveFX();
        lastUseTime = Time.time;

        Debug.Log("Shockwave ultimate used. Hit enemies: " + enemies.Length);
    }

    private int GetFinalDamage()
    {
        int finalDamage = damage;

        if (PlayerStats.Instance != null)
        {
            finalDamage = PlayerStats.Instance.GetTotalDamage(damage);
        }

        return finalDamage;
    }

    private bool CanUse()
    {
        if (Time.time < lastUseTime + cooldown)
        {
            Debug.Log("Shockwave cooldown");
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    private void SpawnShockwaveFX()
    {
        if (shockwaveFxPrefab == null) return;

        GameObject fx = Instantiate(
            shockwaveFxPrefab,
            transform.position,
            Quaternion.identity
        );

        ShockwaveWaveFX waveFX = fx.GetComponent<ShockwaveWaveFX>();

        if (waveFX != null)
        {
            waveFX.targetRadius = radius;
        }
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