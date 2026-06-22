using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class NineDigitDoorUI : MonoBehaviour
{
    public static NineDigitDoorUI Instance;

    [Header("UI")]
    public GameObject panel;
    public TMP_InputField codeInput;

    [Header("Disable While Open")]
    public MonoBehaviour[] componentsToDisableWhileOpen;

    private static NineDigitPanel activePanel;

    private bool isOpen = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isOpen) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCode();
        }
    }

    public void OpenPanel(NineDigitPanel panelToOpen)
    {
        Debug.Log("OPEN PANEL");

        activePanel = panelToOpen;
        isOpen = true;

        if (panel != null)
        {
            panel.SetActive(true);
        }

        DisableGameplayComponents();

        Time.timeScale = 0f;

        if (codeInput != null)
        {
            codeInput.text = "";

            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(codeInput.gameObject);
            }

            codeInput.ActivateInputField();
            codeInput.Select();
        }
        else
        {
            Debug.LogError("NineDigitDoorUI: codeInput is not assigned");
        }
    }

    public void SubmitCode()
    {
        Debug.Log("BUTTON WORKS");

        if (activePanel == null)
        {
            Debug.LogError("ACTIVE PANEL IS NULL");
            return;
        }

        if (codeInput == null)
        {
            Debug.LogError("NineDigitDoorUI: codeInput is not assigned");
            return;
        }

        string enteredCode = codeInput.text.Trim();

        Debug.Log("ENTERED: " + enteredCode);

        if (enteredCode.Length != 9)
        {
            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Введите 9 цифр");
            }

            return;
        }

        activePanel.CheckCode(enteredCode);

        ClosePanel();
    }

    public void ClosePanel()
    {
        Debug.Log("CLOSE PANEL");

        isOpen = false;

        if (panel != null)
        {
            panel.SetActive(false);
        }

        Time.timeScale = 1f;

        EnableGameplayComponents();

        activePanel = null;
    }

    private void DisableGameplayComponents()
    {
        if (componentsToDisableWhileOpen == null) return;

        foreach (MonoBehaviour component in componentsToDisableWhileOpen)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }

    private void EnableGameplayComponents()
    {
        if (componentsToDisableWhileOpen == null) return;

        foreach (MonoBehaviour component in componentsToDisableWhileOpen)
        {
            if (component != null)
            {
                component.enabled = true;
            }
        }
    }
}