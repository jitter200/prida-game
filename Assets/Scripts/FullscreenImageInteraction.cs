using UnityEngine;
using UnityEngine.UI;

public class FullscreenImageInteraction : MonoBehaviour
{
    [Header("Interaction")]
    public KeyCode interactKey = KeyCode.E;
    public float interactDistance = 2f;

    [Header("Player")]
    public Transform player;

    [Header("UI")]
    public GameObject imagePanel;
    public Image fullscreenImage;

    [Header("Sprite")]
    public Sprite imageToShow;

    private bool playerInRange;
    private bool isOpened;

    private void Start()
    {
        if (imagePanel != null)
        {
            imagePanel.SetActive(false);
        }

        if (fullscreenImage != null)
        {
            fullscreenImage.sprite = imageToShow;
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        playerInRange = distance <= interactDistance;

        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            ToggleImage();
        }
    }

    private void ToggleImage()
    {
        isOpened = !isOpened;

        imagePanel.SetActive(isOpened);

        if (isOpened)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}