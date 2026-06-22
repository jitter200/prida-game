using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadoutSelectionUI : MonoBehaviour
{
    public static LoadoutSelectionUI Instance;

    [Header("Panel")]
    public GameObject panel;

    [Header("Weapon Buttons")]
    public Button swordButton;
    public Button heavySwordButton;
    public Button bowButton;
    public Button spearButton;

    [Header("Ultimate Buttons")]
    public Button rewindButton;
    public Button shockwaveButton;
    public Button healButton;
    public Button lightningButton;

    [Header("Text")]
    public TextMeshProUGUI selectedText;

    private string selectedWeapon;
    private string selectedUltimate;
    private string nextSceneName;

    private void Awake()
    {
        Instance = this;

        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public void Open(string sceneToLoad)
    {
        if (PlayerLoadout.Instance == null)
        {
            Debug.LogWarning("PlayerLoadout not found");
            SceneManager.LoadScene(sceneToLoad);
            return;
        }

        nextSceneName = sceneToLoad;

        selectedWeapon = PlayerLoadout.Instance.currentWeapon;
        selectedUltimate = PlayerLoadout.Instance.currentUltimate;

        panel.SetActive(true);
        Time.timeScale = 0f;

        RefreshButtons();
        UpdateSelectedText();
    }

    public void SelectWeapon(string weaponName)
    {
        if (PlayerLoadout.Instance == null) return;

        if (!PlayerLoadout.Instance.ownedWeapons.Contains(weaponName))
        {
            ShowMessage("Оружие не открыто: " + weaponName);
            return;
        }

        selectedWeapon = weaponName;
        UpdateSelectedText();
    }

    public void SelectUltimate(string ultimateName)
    {
        if (PlayerLoadout.Instance == null) return;

        if (!PlayerLoadout.Instance.ownedUltimates.Contains(ultimateName))
        {
            ShowMessage("Ульта не открыта: " + ultimateName);
            return;
        }

        selectedUltimate = ultimateName;
        UpdateSelectedText();
    }

    public void ContinueToNextScene()
    {
        if (PlayerLoadout.Instance != null)
        {
            PlayerLoadout.Instance.EquipWeapon(selectedWeapon);
            PlayerLoadout.Instance.EquipUltimate(selectedUltimate);
        }

        Time.timeScale = 1f;
        panel.SetActive(false);

        SceneManager.LoadScene(nextSceneName);
    }

    public void Cancel()
    {
        Time.timeScale = 1f;
        panel.SetActive(false);
    }

    private void RefreshButtons()
    {
        SetButtonVisible(swordButton, "Sword", true);
        SetButtonVisible(heavySwordButton, "HeavySword", true);
        SetButtonVisible(bowButton, "Bow", true);
        SetButtonVisible(spearButton, "Spear", true);

        SetButtonVisible(rewindButton, "Rewind", false);
        SetButtonVisible(shockwaveButton, "Shockwave", false);
        SetButtonVisible(healButton, "Heal", false);
        SetButtonVisible(lightningButton, "Lightning", false);
    }

    private void SetButtonVisible(Button button, string itemName, bool isWeapon)
    {
        if (button == null) return;
        if (PlayerLoadout.Instance == null) return;

        bool isOwned = isWeapon
            ? PlayerLoadout.Instance.ownedWeapons.Contains(itemName)
            : PlayerLoadout.Instance.ownedUltimates.Contains(itemName);

        button.gameObject.SetActive(isOwned);
    }

    private void UpdateSelectedText()
    {
        if (selectedText == null) return;

        selectedText.text =
            "Выбрано оружие: " + selectedWeapon +
            "\nВыбрана ульта: " + selectedUltimate;
    }

    private void ShowMessage(string message)
    {
        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage(message);
        }

        Debug.Log(message);
    }
}