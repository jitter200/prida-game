using UnityEngine;

public class Ultimate_Heal : MonoBehaviour
{
    [Header("Heal Settings")]
    public int healAmount = 5;
    public float cooldown = 15f;

    [Header("Plus FX")]
    public GameObject healPlusFxPrefab;
    public Vector3 healPlusFxOffset = Vector3.zero;

    [Header("Aura FX")]
    public GameObject healAuraFxPrefab;
    public Vector3 healAuraFxOffset = Vector3.zero;

    [Header("FX Settings")]
    public bool attachFxToPlayer = true;

    private float lastUseTime = -999f;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    public void Use()
    {
        if (!CanUse()) return;

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }

        SpawnHealFX();

        lastUseTime = Time.time;

        Debug.Log("Heal ultimate used");
    }

    private void SpawnHealFX()
    {
        SpawnFX(healPlusFxPrefab, healPlusFxOffset);
        SpawnFX(healAuraFxPrefab, healAuraFxOffset);
    }

    private void SpawnFX(GameObject prefab, Vector3 offset)
    {
        if (prefab == null) return;

        GameObject fx;

        if (attachFxToPlayer)
        {
            fx = Instantiate(prefab, transform);
            fx.transform.localPosition = offset;
            fx.transform.localRotation = Quaternion.identity;
            fx.transform.localScale = Vector3.one;
        }
        else
        {
            fx = Instantiate(
                prefab,
                transform.position + offset,
                Quaternion.identity
            );
        }
    }

    private bool CanUse()
    {
        if (Time.time < lastUseTime + cooldown)
        {
            Debug.Log("Heal ultimate cooldown");
            return false;
        }

        return true;
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