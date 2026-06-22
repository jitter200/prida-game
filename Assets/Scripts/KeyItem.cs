using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Key Settings")]
    public int keyAmount = 1;

    [Header("Visual")]
    public bool destroyOnPickup = true;

    private bool pickedUp = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (pickedUp) return;
        if (!collision.CompareTag("Player")) return;

        if (PlayerStats.Instance == null)
        {
            Debug.LogWarning("PlayerStats not found");
            return;
        }

        pickedUp = true;

        PlayerStats.Instance.AddKey(keyAmount);

        Debug.Log("Key picked up. Amount: " + keyAmount);

        if (destroyOnPickup)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}