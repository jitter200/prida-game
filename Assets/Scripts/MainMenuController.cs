using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject difficultyPanel;

    [Header("Scenes")]
    public string storySceneName = "IntroCutscene";
    public string challengeSceneName = "ChallengeMode";

    [Header("New Game Plus")]
    public GameObject newGamePlusButton;

    private void Awake()
    {
        if (newGamePlusButton != null)
        {
            newGamePlusButton.SetActive(false);
        }
    }

    private void Start()
    {
        ShowMainPanel();
        UpdateNewGamePlusButton();
    }

    public void ShowMainPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(true);

        if (difficultyPanel != null)
            difficultyPanel.SetActive(false);

        UpdateNewGamePlusButton();
    }

    public void ShowDifficultyPanel()
    {
        if (mainPanel != null)
            mainPanel.SetActive(false);

        if (difficultyPanel != null)
            difficultyPanel.SetActive(true);
    }

    public void StartEasy()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetEasy();
        }

        SceneManager.LoadScene(storySceneName);
    }

    public void StartNormal()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetNormal();
        }

        SceneManager.LoadScene(storySceneName);
    }

    public void StartHard()
    {
        if (DifficultyManager.Instance != null)
        {
            DifficultyManager.Instance.SetHard();
        }

        SceneManager.LoadScene(storySceneName);
    }

    public void StartChallengeMode()
    {
        SceneManager.LoadScene(challengeSceneName);
    }

    public void StartNewGamePlus()
    {
        if (GameProgress.Instance != null)
        {
            GameProgress.Instance.StartNewGamePlus();
        }

        ShowDifficultyPanel();
    }

    public void ExitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }

    private void UpdateNewGamePlusButton()
    {
        if (newGamePlusButton == null)
        {
            Debug.LogWarning("New Game Plus Button не назначена в MainMenuController");
            return;
        }

        bool unlocked = GameProgress.Instance != null &&
                        GameProgress.Instance.IsGameCompleted;

        Debug.Log("New Game+ unlocked: " + unlocked);

        newGamePlusButton.SetActive(unlocked);
    }
}