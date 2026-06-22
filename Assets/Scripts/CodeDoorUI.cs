using TMPro;
using UnityEngine;

public class CodeDoorUI : MonoBehaviour
{
    public static CodeDoorUI Instance;
    public TextMeshProUGUI knownDigitsText;
    [Header("UI")]
    public GameObject panel;
    public TMP_InputField codeInput;

    private CodePanel currentPanel;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (panel != null && panel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public void OpenPanel(CodePanel panelToOpen)
    {
        currentPanel = panelToOpen;

        panel.SetActive(true);
        Time.timeScale = 0f;

        codeInput.text = "";
        codeInput.ActivateInputField();

        UpdateKnownDigitsText();
    }

    public void SubmitCode()
    {
        if (currentPanel == null) return;

        string enteredCode = codeInput.text.Trim();

        if (enteredCode.Length != 4)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Код должен быть из 4 цифр");
            }

            return;
        }

        currentPanel.CheckCode(enteredCode);
        ClosePanel();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
        currentPanel = null;
    }
    private void UpdateKnownDigitsText()
    {
        if (knownDigitsText == null) return;

        string knownCode = "****";

        if (CodeProgress.Instance != null)
        {
            knownCode = CodeProgress.Instance.GetKnownCode();
        }

        knownDigitsText.text = "Известные цифры: " + knownCode;
    }
}