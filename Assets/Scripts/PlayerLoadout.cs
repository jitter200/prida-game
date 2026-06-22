using System.Collections.Generic;
using UnityEngine;

public class PlayerLoadout : MonoBehaviour
{
    public static PlayerLoadout Instance;

    public List<string> ownedWeapons = new List<string>();
    public List<string> ownedUltimates = new List<string>();

    public string currentWeapon = "Sword";
    public string currentUltimate = "Rewind";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            AddDefaultWeapon();
            AddDefaultUltimate();

            TryApplyNewGamePlusLoadout();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void AddDefaultWeapon()
    {
        if (!ownedWeapons.Contains("Sword"))
        {
            ownedWeapons.Add("Sword");
        }

        if (string.IsNullOrEmpty(currentWeapon))
        {
            currentWeapon = "Sword";
        }
    }

    private void AddDefaultUltimate()
    {
        if (!ownedUltimates.Contains("Rewind"))
        {
            ownedUltimates.Add("Rewind");
        }

        if (string.IsNullOrEmpty(currentUltimate))
        {
            currentUltimate = "Rewind";
        }
    }

    public void AddWeapon(string weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
            Debug.Log("Weapon unlocked: " + weapon);
        }
    }

    public void AddUltimate(string ultimate)
    {
        if (!ownedUltimates.Contains(ultimate))
        {
            ownedUltimates.Add(ultimate);
            Debug.Log("Ultimate unlocked: " + ultimate);
        }
    }

    public void EquipWeapon(string weapon)
    {
        if (!ownedWeapons.Contains(weapon))
        {
            Debug.LogWarning("Weapon not owned: " + weapon);
            return;
        }

        currentWeapon = weapon;
        Debug.Log("Equipped weapon: " + weapon);
    }

    public void EquipUltimate(string ultimate)
    {
        if (!ownedUltimates.Contains(ultimate))
        {
            Debug.LogWarning("Ultimate not owned: " + ultimate);
            return;
        }

        currentUltimate = ultimate;
        Debug.Log("Equipped ultimate: " + ultimate);
    }

    public void UnlockAllForNewGamePlus()
    {
        AddWeapon("Sword");
        AddWeapon("HeavySword");
        AddWeapon("Bow");
        AddWeapon("Spear");

        AddUltimate("Rewind");
        AddUltimate("Shockwave");
        AddUltimate("Heal");
        AddUltimate("Lightning");

        if (string.IsNullOrEmpty(currentWeapon))
        {
            currentWeapon = "Sword";
        }

        if (string.IsNullOrEmpty(currentUltimate))
        {
            currentUltimate = "Rewind";
        }

        Debug.Log("NG+: all weapons and ultimates unlocked");
    }

    private void TryApplyNewGamePlusLoadout()
    {
        if (GameProgress.Instance == null) return;
        if (!GameProgress.Instance.IsNewGamePlus) return;

        UnlockAllForNewGamePlus();
    }
}