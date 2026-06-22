using TMPro;
using UnityEngine;

public class RareRewardPopup : MonoBehaviour
{
    public static RareRewardPopup Instance;

    public GameObject panel;
    public TextMeshProUGUI rewardText;

    private string currentRewardName;
    private bool isWeaponReward;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        panel.SetActive(false);
    }

    public void ShowWeaponPopup(string weaponName)
    {
        currentRewardName = weaponName;
        isWeaponReward = true;

        rewardText.text = "Получено новое оружие: " + weaponName + "\nЭкипировать сейчас?";
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ShowUltimatePopup(string ultimateName)
    {
        currentRewardName = ultimateName;
        isWeaponReward = false;

        rewardText.text = "Получена новая ульта: " + ultimateName + "\nЭкипировать сейчас?";
        panel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void EquipNow()
    {
        if (isWeaponReward)
        {
            PlayerLoadout.Instance.EquipWeapon(currentRewardName);

            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.EquipWeapon(currentRewardName);
            }
        }
        else
        {
            PlayerLoadout.Instance.EquipUltimate(currentRewardName);
        }

        ClosePopup();
    }

    public void DontEquip()
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}