using UnityEngine;

public class NineDigitDoor : MonoBehaviour
{
    [Header("Door Collider")]
    public Collider2D solidCollider;

    [Header("Visual")]
    public SpriteRenderer doorSpriteRenderer;
    public Sprite openedSprite;

    [Header("State")]
    public bool isOpen = false;

    public void OpenDoor()
    {
        if (isOpen) return;

        isOpen = true;

        if (solidCollider != null)
        {
            solidCollider.enabled = false;
        }

        if (doorSpriteRenderer != null && openedSprite != null)
        {
            doorSpriteRenderer.sprite = openedSprite;
        }

        Debug.Log("9 digit door opened");
    }
}