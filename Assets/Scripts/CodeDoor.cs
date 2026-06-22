using UnityEngine;

public class CodeDoor : MonoBehaviour
{
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

    [Header("State")]
    public bool isOpen = false;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        PlayOpenSound();

        if (solidCollider != null)
        {
            solidCollider.enabled = false;
        }

        if (doorSpriteRenderer != null && openedSprite != null)
        {
            doorSpriteRenderer.sprite = openedSprite;
        }

        Debug.Log("Code door opened");
    }

    private void PlayOpenSound()
    {
        if (audioSource == null) return;
        if (openDoorSound == null) return;

        audioSource.PlayOneShot(openDoorSound, openDoorVolume);
    }
}