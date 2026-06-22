using UnityEngine;

public class Chest : MonoBehaviour
{
    public enum RareRewardType
    {
        Weapon,
        Ultimate
    }

    [Header("Chest Settings")]
    public bool isRareChest = false;
    private bool isOpened = false;
    private bool playerInRange = false;

    [Header("Audio")]
    public AudioClip openChestSound;

    [Range(0f, 1f)]
    public float openChestVolume = 0.8f;

    [Header("Normal Rewards")]
    public int normalCoins = 10;
    public int normalDamageBonus = 1;
    public float normalSpeedBonus = 0.5f;
    public int normalMaxHealthBonus = 1;

    [Header("Rare Rewards")]
    public int rareCoins = 30;
    public int rareDamageBonus = 3;
    public float rareSpeedBonus = 1f;
    public int rareMaxHealthBonus = 3;

    [Header("Rare Unique Reward")]
    public RareRewardType rareRewardType = RareRewardType.Weapon;
    public string uniqueRewardName = "HeavySword";

    private void Update()
    {
        if (playerInRange && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        if (isOpened) return;

        isOpened = true;

        PlayOpenChestSound();

        if (isRareChest)
        {
            GiveRareRewards();
        }
        else
        {
            GiveNormalRewards();
        }

        Debug.Log("Chest opened");

        gameObject.SetActive(false);
    }

    private void PlayOpenChestSound()
    {
        if (openChestSound == null) return;

        GameObject soundObject = new GameObject("ChestOpenSound");
        soundObject.transform.position = transform.position;

        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = openChestSound;
        audioSource.volume = openChestVolume;
        audioSource.loop = false;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;

        audioSource.Play();

        Destroy(soundObject, openChestSound.length + 0.1f);
    }

    private void GiveNormalRewards()
    {
        int finalCoins = normalCoins;

        if (DifficultyManager.Instance != null)
        {
            finalCoins = DifficultyManager.Instance.GetRewardAmount(normalCoins);
        }

        PlayerStats.Instance.AddCoins(finalCoins);
        PlayerStats.Instance.AddDamage(normalDamageBonus);
        PlayerStats.Instance.AddSpeed(normalSpeedBonus);
        PlayerStats.Instance.AddMaxHealth(normalMaxHealthBonus);
    }

    private void GiveRareRewards()
    {
        int finalCoins = rareCoins;

        if (DifficultyManager.Instance != null)
        {
            finalCoins = DifficultyManager.Instance.GetRewardAmount(rareCoins);
        }

        PlayerStats.Instance.AddCoins(finalCoins);
        PlayerStats.Instance.AddDamage(rareDamageBonus);
        PlayerStats.Instance.AddSpeed(rareSpeedBonus);
        PlayerStats.Instance.AddMaxHealth(rareMaxHealthBonus);

        if (rareRewardType == RareRewardType.Weapon)
        {
            PlayerLoadout.Instance.AddWeapon(uniqueRewardName);
            RareRewardPopup.Instance.ShowWeaponPopup(uniqueRewardName);
        }
        else
        {
            PlayerLoadout.Instance.AddUltimate(uniqueRewardName);
            RareRewardPopup.Instance.ShowUltimatePopup(uniqueRewardName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;
        Debug.Log("Press E to open chest");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}