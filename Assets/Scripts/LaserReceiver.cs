using UnityEngine;

public class LaserReceiver : MonoBehaviour
{
    [Header("Visual")]
    public SpriteRenderer spriteRenderer;
    public Sprite inactiveSprite;
    public Sprite activeSprite;

    [Header("Reward")]
    public CodeDoor doorToOpen;
    public GameObject objectToEnable;
    public GameObject objectToDisable;

    private bool activated = false;

    private void Awake()
    {
        SetVisual(false);
    }

    public void ReceiveLaser()
    {
        if (activated) return;

        activated = true;
        SetVisual(true);

        if (doorToOpen != null)
        {
            doorToOpen.OpenDoor();
        }

        if (objectToEnable != null)
        {
            objectToEnable.SetActive(true);
        }

        if (objectToDisable != null)
        {
            objectToDisable.SetActive(false);
        }

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Механизм активирован");
        }

        Debug.Log("Laser receiver activated");
    }

    private void SetVisual(bool active)
    {
        if (spriteRenderer == null) return;

        if (active && activeSprite != null)
        {
            spriteRenderer.sprite = activeSprite;
        }
        else if (!active && inactiveSprite != null)
        {
            spriteRenderer.sprite = inactiveSprite;
        }
    }
}