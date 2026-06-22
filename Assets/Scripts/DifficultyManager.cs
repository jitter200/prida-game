using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    [Header("Current Difficulty")]
    public Difficulty currentDifficulty = Difficulty.Normal;

    [Header("Enemy Multipliers")]
    public float easyEnemyHealthMultiplier = 0.75f;
    public float easyEnemyDamageMultiplier = 0.75f;

    public float normalEnemyHealthMultiplier = 1f;
    public float normalEnemyDamageMultiplier = 1f;

    public float hardEnemyHealthMultiplier = 1.5f;
    public float hardEnemyDamageMultiplier = 1.5f;

    [Header("MiniBoss Multipliers")]
    public float easyMiniBossHealthMultiplier = 0.75f;
    public float easyMiniBossDamageMultiplier = 0.75f;

    public float normalMiniBossHealthMultiplier = 1f;
    public float normalMiniBossDamageMultiplier = 1f;

    public float hardMiniBossHealthMultiplier = 1.5f;
    public float hardMiniBossDamageMultiplier = 1.5f;

    [Header("Boss Multipliers")]
    public float easyBossHealthMultiplier = 0.75f;
    public float easyBossDamageMultiplier = 0.75f;

    public float normalBossHealthMultiplier = 1f;
    public float normalBossDamageMultiplier = 1f;

    public float hardBossHealthMultiplier = 1.5f;
    public float hardBossDamageMultiplier = 1.5f;

    [Header("Trap Multipliers")]
    public float easyTrapDamageMultiplier = 0.75f;
    public float normalTrapDamageMultiplier = 1f;
    public float hardTrapDamageMultiplier = 1.5f;

    [Header("Reward Multipliers")]
    public float easyRewardMultiplier = 1.25f;
    public float normalRewardMultiplier = 1f;
    public float hardRewardMultiplier = 1.5f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetEasy()
    {
        currentDifficulty = Difficulty.Easy;
        Debug.Log("Difficulty set to Easy");
    }

    public void SetNormal()
    {
        currentDifficulty = Difficulty.Normal;
        Debug.Log("Difficulty set to Normal");
    }

    public void SetHard()
    {
        currentDifficulty = Difficulty.Hard;
        Debug.Log("Difficulty set to Hard");
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        currentDifficulty = difficulty;
        Debug.Log("Difficulty set to " + difficulty);
    }

    public int GetEnemyHealth(int baseHealth)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseHealth * GetEnemyHealthMultiplier()));
    }

    public int GetEnemyDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * GetEnemyDamageMultiplier()));
    }

    public int GetMiniBossHealth(int baseHealth)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseHealth * GetMiniBossHealthMultiplier()));
    }

    public int GetMiniBossDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * GetMiniBossDamageMultiplier()));
    }

    public int GetBossHealth(int baseHealth)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseHealth * GetBossHealthMultiplier()));
    }

    public int GetBossDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * GetBossDamageMultiplier()));
    }

    public int GetTrapDamage(int baseDamage)
    {
        return Mathf.Max(1, Mathf.RoundToInt(baseDamage * GetTrapDamageMultiplier()));
    }

    public int GetRewardAmount(int baseAmount)
    {
        return Mathf.Max(0, Mathf.RoundToInt(baseAmount * GetRewardMultiplier()));
    }

    private float GetEnemyHealthMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyEnemyHealthMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardEnemyHealthMultiplier;

        return normalEnemyHealthMultiplier;
    }

    private float GetEnemyDamageMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyEnemyDamageMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardEnemyDamageMultiplier;

        return normalEnemyDamageMultiplier;
    }

    private float GetMiniBossHealthMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyMiniBossHealthMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardMiniBossHealthMultiplier;

        return normalMiniBossHealthMultiplier;
    }

    private float GetMiniBossDamageMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyMiniBossDamageMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardMiniBossDamageMultiplier;

        return normalMiniBossDamageMultiplier;
    }

    private float GetBossHealthMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyBossHealthMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardBossHealthMultiplier;

        return normalBossHealthMultiplier;
    }

    private float GetBossDamageMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyBossDamageMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardBossDamageMultiplier;

        return normalBossDamageMultiplier;
    }

    private float GetTrapDamageMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyTrapDamageMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardTrapDamageMultiplier;

        return normalTrapDamageMultiplier;
    }

    private float GetRewardMultiplier()
    {
        if (currentDifficulty == Difficulty.Easy) return easyRewardMultiplier;
        if (currentDifficulty == Difficulty.Hard) return hardRewardMultiplier;

        return normalRewardMultiplier;
    }
}