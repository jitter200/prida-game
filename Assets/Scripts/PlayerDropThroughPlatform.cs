using System.Collections;
using UnityEngine;

public class PlayerDropThroughPlatform : MonoBehaviour
{
    [Header("Input")]
    public KeyCode dropKey = KeyCode.S;

    [Header("Settings")]
    public float disableCollisionTime = 0.4f;

    private Collider2D playerCollider;
    private bool isDropping;

    private void Awake()
    {
        playerCollider = GetComponent<Collider2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDropping) return;

        if (!collision.gameObject.CompareTag("DropPlatform")) return;

        if (Input.GetKeyDown(dropKey))
        {
            StartCoroutine(DropThrough(collision.collider));
        }
    }

    private IEnumerator DropThrough(Collider2D platformCollider)
    {
        isDropping = true;

        Physics2D.IgnoreCollision(playerCollider, platformCollider, true);

        yield return new WaitForSeconds(disableCollisionTime);

        Physics2D.IgnoreCollision(playerCollider, platformCollider, false);

        isDropping = false;
    }
}