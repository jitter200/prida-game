using UnityEngine;

public class CarryableBox : MonoBehaviour
{
    [Header("State")]
    public bool isCarried = false;

    [Header("Puzzle ID")]
    public int boxID = 1;

    private Rigidbody2D rb;
    private Collider2D boxCollider;
    private Transform originalParent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<Collider2D>();
        originalParent = transform.parent;
    }

    public void PickUp(Transform carryPoint)
    {
        isCarried = true;

        transform.SetParent(carryPoint);
        transform.localPosition = Vector3.zero;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.simulated = false;
        }

        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }
    }

    public void Drop()
    {
        isCarried = false;

        transform.SetParent(originalParent);

        if (boxCollider != null)
        {
            boxCollider.enabled = true;
        }

        if (rb != null)
        {
            rb.simulated = true;
            rb.gravityScale = 3f;
            rb.velocity = Vector2.zero;
        }
    }
}