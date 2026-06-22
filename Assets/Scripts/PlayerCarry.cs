using UnityEngine;

public class PlayerCarry : MonoBehaviour
{
    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Carry")]
    public Transform carryPoint;
    public float pickupRange = 1.2f;
    public LayerMask boxLayer;

    private CarryableBox carriedBox;

    private void Update()
    {
        if (IsInteractPressed())
        {
            if (carriedBox == null)
            {
                TryPickUpBox();
            }
            else
            {
                DropBox();
            }
        }
    }

    private bool IsInteractPressed()
    {
        return Input.GetKeyDown(interactKey) ||
               (MobileInput.Instance != null && MobileInput.Instance.interactPressed);
    }

    private void TryPickUpBox()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            pickupRange,
            boxLayer
        );

        if (hits.Length == 0)
        {
            return;
        }

        CarryableBox nearestBox = null;
        float nearestDistance = Mathf.Infinity;

        foreach (Collider2D hit in hits)
        {
            CarryableBox box = hit.GetComponent<CarryableBox>();

            if (box == null)
            {
                box = hit.GetComponentInParent<CarryableBox>();
            }

            if (box == null || box.isCarried)
            {
                continue;
            }

            float distance = Vector2.Distance(transform.position, box.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestBox = box;
            }
        }

        if (nearestBox != null)
        {
            carriedBox = nearestBox;
            carriedBox.PickUp(carryPoint);

            if (UIMessage.Instance != null)
            {
                UIMessage.Instance.ShowMessage("Коробка поднята");
            }
        }
    }

    private void DropBox()
    {
        if (carriedBox == null) return;

        carriedBox.Drop();
        carriedBox = null;

        if (UIMessage.Instance != null)
        {
            UIMessage.Instance.ShowMessage("Коробка поставлена");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}