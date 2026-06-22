using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Object")]
    public GameObject doorObject;

    [Header("Door Collider")]
    public Collider2D solidCollider;

    [Header("Visual")]
    public SpriteRenderer doorSpriteRenderer;
    public Sprite openedSprite;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openDoorSound;

    [Range(0f, 1f)]
    public float openDoorVolume = 0.8f;

    [Header("Door Settings")]
    public bool requiresKeys = false;
    public int requiredKeys = 1;
    public bool spendKeysOnOpen = true;

    [Header("State")]
    public bool isOpen = false;

    private bool playerInRange = false;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if (playerInRange && !isOpen && Input.GetKeyDown(KeyCode.E))
        {
            TryOpen();
        }
    }

    private void TryOpen()
    {
        if (requiresKeys)
        {
            if (PlayerStats.Instance == null)
            {
                Debug.LogWarning("PlayerStats not found");
                return;
            }

            if (!PlayerStats.Instance.HasKeys(requiredKeys))
            {
                Debug.Log("Need keys: " + requiredKeys);

                if (UIMessage.Instance != null)
                {
                    UIMessage.Instance.ShowMessage("Требуется ключей: " + requiredKeys);
                }

                return;
            }

            if (spendKeysOnOpen)
            {
                PlayerStats.Instance.SpendKeys(requiredKeys);
            }
        }

        OpenDoor();
    }

    public void ForceOpen()
    {
        if (isOpen) return;

        OpenDoor();
    }

    private void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        Debug.Log("Door opened");

        PlayOpenSound();

        if (solidCollider != null)
        {
            solidCollider.enabled = false;
        }

        if (doorSpriteRenderer != null && openedSprite != null)
        {
            doorSpriteRenderer.sprite = openedSprite;
        }

        
    }

    private void PlayOpenSound()
    {
        if (audioSource == null) return;
        if (openDoorSound == null) return;

        audioSource.PlayOneShot(openDoorSound, openDoorVolume);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = true;

        if (requiresKeys)
        {
            Debug.Log("Press E to open. Required keys: " + requiredKeys);
        }
        else
        {
            Debug.Log("Press E to open door");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
    }
}