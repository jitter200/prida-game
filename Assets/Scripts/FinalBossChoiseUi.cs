using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FinalBossChoiceUI : MonoBehaviour
{
    public static FinalBossChoiceUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI choiceText;
    public Button killButton;
    public Button spareButton;

    [Header("Ending Slideshow")]
    public EndingSlideshowUI endingSlideshowUI;

    [Header("Text")]
    public string questionText = "Реши судьбу босса";

    private FinalBossHealth currentBoss;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (killButton != null)
        {
            killButton.onClick.RemoveAllListeners();
            killButton.onClick.AddListener(KillBoss);
        }

        if (spareButton != null)
        {
            spareButton.onClick.RemoveAllListeners();
            spareButton.onClick.AddListener(SpareBoss);
        }
    }

    public void Open(FinalBossHealth boss)
    {
        currentBoss = boss;

        if (choiceText != null)
        {
            choiceText.text = questionText;
        }

        if (panel != null)
        {
            panel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    private void KillBoss()
    {
        ResolveChoice(true);
    }

    private void SpareBoss()
    {
        ResolveChoice(false);
    }

    private void ResolveChoice(bool killBoss)
    {
        Time.timeScale = 1f;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        if (currentBoss != null)
        {
            currentBoss.ResolveFinalChoice(killBoss);
        }

        if (endingSlideshowUI != null)
        {
            endingSlideshowUI.PlayEnding(killBoss);
        }
        else
        {
            Debug.LogWarning("EndingSlideshowUI не назначен в FinalBossChoiceUI");
        }
    }
}