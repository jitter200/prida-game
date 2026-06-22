using UnityEngine;

public class GameProgress : MonoBehaviour
{
    public static GameProgress Instance;

    private const string CompletedKey = "GameCompleted";
    private const string NGPlusActiveKey = "NGPlusActive";

    public bool IsGameCompleted { get; private set; }
    public bool IsNewGamePlus { get; private set; }

    private const string NGPlusUnlockLoadoutKey = "NGPlusUnlockLoadout";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Load();
    }

    private void Load()
    {
        IsGameCompleted = PlayerPrefs.GetInt(CompletedKey, 0) == 1;
        IsNewGamePlus = PlayerPrefs.GetInt(NGPlusActiveKey, 0) == 1;
    }

    public void CompleteGame()
    {
        IsGameCompleted = true;

        PlayerPrefs.SetInt(CompletedKey, 1);
        PlayerPrefs.Save();

        Debug.Log("Game completed. New Game+ unlocked.");
    }

    public void StartNewGamePlus()
    {
        if (!IsGameCompleted)
        {
            Debug.LogWarning("New Game+ is locked.");
            return;
        }

        IsNewGamePlus = true;

        PlayerPrefs.SetInt(NGPlusActiveKey, 1);
        PlayerPrefs.Save();

        if (PlayerLoadout.Instance != null)
        {
            PlayerLoadout.Instance.UnlockAllForNewGamePlus();
        }

        Debug.Log("New Game+ started. All weapons and ultimates unlocked.");
    }

    public float GetNGPlusHealthMultiplier()
    {
        return IsNewGamePlus ? 1.4f : 1f;
    }

    public float GetNGPlusDamageMultiplier()
    {
        return IsNewGamePlus ? 1.25f : 1f;
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(CompletedKey);
        PlayerPrefs.DeleteKey(NGPlusActiveKey);
        PlayerPrefs.Save();

        IsGameCompleted = false;
        IsNewGamePlus = false;
    }
    [ContextMenu("Reset Game Progress")]
    public void ResetProgressFromInspector()
    {
        PlayerPrefs.DeleteKey("GameCompleted");
        PlayerPrefs.DeleteKey("NGPlusActive");
        PlayerPrefs.Save();

        IsGameCompleted = false;
        IsNewGamePlus = false;

        Debug.Log("Game progress reset");
    }
    public bool ShouldUnlockAllLoadoutForNGPlus()
    {
        return PlayerPrefs.GetInt(NGPlusUnlockLoadoutKey, 0) == 1;
    }

    public void MarkNGPlusLoadoutUnlocked()
    {
        PlayerPrefs.DeleteKey(NGPlusUnlockLoadoutKey);
        PlayerPrefs.Save();
    }
}